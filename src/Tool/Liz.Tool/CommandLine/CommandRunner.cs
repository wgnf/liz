using Liz.Core;
using Liz.Core.Logging.Contracts;
using Liz.Core.Progress;
using Liz.Core.Settings;
using Liz.Tool.Contracts.CommandLine;
using Liz.Tool.Logging;
using Liz.Tool.Progress;

namespace Liz.Tool.CommandLine;

internal sealed class CommandRunner : ICommandRunner
{
    private readonly IExtractLicensesFactory _extractLicensesFactory;
    
    public CommandRunner(
        IExtractLicensesFactory? extractLicensesFactory = null)
    {
        _extractLicensesFactory = extractLicensesFactory ?? new ExtractLicensesFactory();
    }
    
    public async Task RunAsync(
        FileInfo targetFile, 
        LogLevel logLevel, 
        bool includeTransitive, 
        bool suppressPrintDetails,
        bool suppressPrintIssues,
        bool suppressProgressbar,
        string? licenseTypeDefinitions,
        string? urlToLicenseTypeMapping,
        string? whitelist,
        string? blacklist,
        string? exportTexts)
    {
        ArgumentNullException.ThrowIfNull(targetFile);

        var settings = CreateSettings(
            targetFile, 
            includeTransitive, 
            suppressPrintDetails, 
            suppressPrintIssues,
            licenseTypeDefinitions,
            urlToLicenseTypeMapping,
            whitelist,
            blacklist,
            exportTexts);

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
        await extractLicenses.ExtractAsync().ConfigureAwait(false);
    }

    private static ExtractLicensesSettingsBase CreateSettings(
        FileSystemInfo targetFile,
        bool includeTransitive,
        bool suppressPrintDetails,
        bool suppressPrintIssues,
        string? licenseTypeDefinitionsFile,
        string? urlToLicenseTypeMappingFile,
        string? whitelistFile,
        string? blacklistFile,
        string? exportTextsDirectory)
    {
        var settings = new ExtractLicensesSettings(targetFile.FullName)
        {
            IncludeTransitiveDependencies = includeTransitive,
            SuppressPrintDetails = suppressPrintDetails,
            SuppressPrintIssues = suppressPrintIssues,
            LicenseTypeDefinitionsFilePath = licenseTypeDefinitionsFile,
            UrlToLicenseTypeMappingFilePath = urlToLicenseTypeMappingFile,
            LicenseTypeWhitelistFilePath = whitelistFile,
            LicenseTypeBlacklistFilePath = blacklistFile,
            ExportLicenseTextsDirectory = exportTextsDirectory
        };

        return settings;
    }
}
