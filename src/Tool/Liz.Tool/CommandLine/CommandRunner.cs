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
    
    public CommandRunner(IExtractLicensesFactory? extractLicensesFactory = null)
    {
        _extractLicensesFactory = extractLicensesFactory ?? new ExtractLicensesFactory();
    }
    
    public async Task RunAsync(
        FileInfo targetFile, 
        LogLevel logLevel, 
        bool includeTransitive, 
        bool suppressPrintDetails,
        bool suppressPrintIssues,
        bool suppressProgressbar)
    {
        ArgumentNullException.ThrowIfNull(targetFile);

        var settings = CreateSettings(targetFile, logLevel, includeTransitive, suppressPrintDetails, suppressPrintIssues);

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
        var extractLicenses = _extractLicensesFactory.Create(settings, loggerProvider, progressHandler);
        await extractLicenses.ExtractAsync();
    }

    private static ExtractLicensesSettingsBase CreateSettings(
        FileSystemInfo targetFile,
        LogLevel logLevel,
        bool includeTransitive,
        bool suppressPrintDetails,
        bool suppressPrintIssues)
    {
        var settings = new ExtractLicensesSettings
        {
            TargetFile = targetFile.FullName,
            LogLevel = logLevel,
            IncludeTransitiveDependencies = includeTransitive,
            SuppressPrintDetails = suppressPrintDetails,
            SuppressPrintIssues = suppressPrintIssues
        };

        return settings;
    }
}
