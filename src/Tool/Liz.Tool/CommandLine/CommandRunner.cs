using Liz.Core;
using Liz.Core.License.Sources.LicenseType;
using Liz.Core.Logging.Contracts;
using Liz.Core.Progress;
using Liz.Core.Settings;
using Liz.Tool.Contracts;
using Liz.Tool.Contracts.CommandLine;
using Liz.Tool.Logging;
using Liz.Tool.Progress;
using System.IO.Abstractions;
using System.Text.Json;

namespace Liz.Tool.CommandLine;

internal sealed class CommandRunner : ICommandRunner
{
    private readonly IFileSystem _fileSystem;
    private readonly IExtractLicensesFactory _extractLicensesFactory;
    
    public CommandRunner(
        IExtractLicensesFactory? extractLicensesFactory = null,
        IFileSystem? fileSystem = null)
    {
        _extractLicensesFactory = extractLicensesFactory ?? new ExtractLicensesFactory();
        _fileSystem = fileSystem ?? new FileSystem();
    }
    
    public async Task RunAsync(
        FileInfo targetFile, 
        LogLevel logLevel, 
        bool includeTransitive, 
        bool suppressPrintDetails,
        bool suppressPrintIssues,
        bool suppressProgressbar,
        FileInfo? licenseTypeDefinitions)
    {
        ArgumentNullException.ThrowIfNull(targetFile);

        var licenseTypeDefinitionsFile = licenseTypeDefinitions == null
            ? null
            : _fileSystem.FileInfo.FromFileName(licenseTypeDefinitions.FullName);

        var settings = await CreateSettingsAsync(
            targetFile, 
            includeTransitive, 
            suppressPrintDetails, 
            suppressPrintIssues,
            licenseTypeDefinitionsFile);

        ILoggerProvider? loggerProvider;
        IProgressHandler? progressHandler;

        var commandLineLoggerProvider = new CommandLineLoggerProvider();
        
        if (suppressProgressbar)
        {
            loggerProvider = commandLineLoggerProvider;
            progressHandler = null;
        }
        else
        {
            loggerProvider = new ProgressBarLoggerProvider(commandLineLoggerProvider);
            progressHandler = (IProgressHandler) loggerProvider.Get(logLevel);
        }
        var extractLicenses = _extractLicensesFactory.Create(settings, loggerProvider, logLevel, progressHandler);
        await extractLicenses.ExtractAsync();
    }

    private async static Task<ExtractLicensesSettingsBase> CreateSettingsAsync(
        FileSystemInfo targetFile,
        bool includeTransitive,
        bool suppressPrintDetails,
        bool suppressPrintIssues,
        IFileInfo? licenseTypeDefinitionsFile)
    {
        var settings = new ExtractLicensesSettings
        {
            TargetFile = targetFile.FullName,
            IncludeTransitiveDependencies = includeTransitive,
            SuppressPrintDetails = suppressPrintDetails,
            SuppressPrintIssues = suppressPrintIssues,
            LicenseTypeDefinitions = await GetLicenseTypeDefinitionsFromFileAsync(licenseTypeDefinitionsFile)
        };

        return settings;
    }

    private async static Task<List<LicenseTypeDefinition>> GetLicenseTypeDefinitionsFromFileAsync(IFileInfo? licenseTypeDefinitionsFile)
    {
        if (licenseTypeDefinitionsFile == null) return new List<LicenseTypeDefinition>();
        
        if (!licenseTypeDefinitionsFile.Exists)
            throw new FileNotFoundException("the provided license-type-definitions-file does not exist!");

        if (!licenseTypeDefinitionsFile.Extension.Contains("json", StringComparison.InvariantCultureIgnoreCase))
            throw new InvalidOperationException("only JSON files are supported for the license-type-definitions-file");

        try
        {
            await using var fileStream = licenseTypeDefinitionsFile.OpenRead();
            var typeDefinitions = await JsonSerializer.DeserializeAsync<List<JsonLicenseTypeDefinition>>(fileStream);

            return typeDefinitions?
                .Where(typeDefinition => !string.IsNullOrWhiteSpace(typeDefinition.LicenseType))
                .Where(typeDefinition => typeDefinition.InclusiveTextSnippets.Any())
                .Select(typeDefinition => new LicenseTypeDefinition(
                    typeDefinition.LicenseType,
                    typeDefinition.InclusiveTextSnippets.ToArray())
                {
                    ExclusiveTextSnippets = typeDefinition.ExclusiveTextSnippets
                })
                .ToList() ?? new List<LicenseTypeDefinition>();
        }
        catch (Exception exception)
        {
            throw new InvalidOperationException("Error while reading license-type-definitions-file", exception);
        }

    }
}
