using DotnetNugetLicenses.Core;
using DotnetNugetLicenses.Core.Logging;
using DotnetNugetLicenses.Core.Settings;
using DotnetNugetLicenses.Tool.Contracts.CommandLine;
using DotnetNugetLicenses.Tool.Logging;
using System;
using System.IO;
using System.Threading.Tasks;

namespace DotnetNugetLicenses.Tool.CommandLine;

internal sealed class CommandRunner : ICommandRunner
{
    private readonly IExtractLicensesFactory _extractLicensesFactory;
    
    public CommandRunner(IExtractLicensesFactory extractLicensesFactory = null)
    {
        _extractLicensesFactory = extractLicensesFactory ?? new ExtractLicensesFactory();
    }
    
    public async Task RunAsync(FileInfo targetFile, LogLevel logLevel, bool includeTransitive)
    {
        if (targetFile == null) throw new ArgumentNullException(nameof(targetFile));

        var settings = CreateSettings(targetFile, logLevel, includeTransitive);
        var loggerProvider = new CommandLineLoggerProvider();
        
        var extractLicenses = _extractLicensesFactory.Create(settings, loggerProvider);
        await extractLicenses.ExtractAsync();
    }

    private static ExtractLicensesSettings CreateSettings(
        FileSystemInfo targetFile,
        LogLevel logLevel,
        bool includeTransitive)
    {
        var settings = new ExtractLicensesSettings(targetFile.FullName)
        {
            LogLevel = logLevel,
            IncludeTransitiveDependencies = includeTransitive
        };

        return settings;
    }
}
