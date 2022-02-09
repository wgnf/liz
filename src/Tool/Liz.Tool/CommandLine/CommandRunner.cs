using Liz.Core;
using Liz.Core.Logging.Contracts;
using Liz.Core.Settings;
using Liz.Tool.Contracts.CommandLine;
using Liz.Tool.Logging;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Liz.Tool.CommandLine;

internal sealed class CommandRunner : ICommandRunner
{
    private readonly IExtractLicensesFactory _extractLicensesFactory;
    
    public CommandRunner(IExtractLicensesFactory extractLicensesFactory = null)
    {
        _extractLicensesFactory = extractLicensesFactory ?? new ExtractLicensesFactory();
    }
    
    public async Task RunAsync(
        FileInfo targetFile, 
        LogLevel logLevel, 
        bool includeTransitive, 
        bool suppressPrintDetails,
        bool suppressPrintIssues)
    {
        ArgumentNullException.ThrowIfNull(targetFile);

        var settings = CreateSettings(targetFile, logLevel, includeTransitive, suppressPrintDetails, suppressPrintIssues);
        var loggerProvider = new CommandLineLoggerProvider();
        
        var extractLicenses = _extractLicensesFactory.Create(settings, loggerProvider);
        await extractLicenses.ExtractAsync();
    }

    private static ExtractLicensesSettings CreateSettings(
        FileSystemInfo targetFile,
        LogLevel logLevel,
        bool includeTransitive,
        bool suppressPrintDetails,
        bool suppressPrintIssues)
    {
        var settings = new ExtractLicensesSettings(targetFile.FullName)
        {
            LogLevel = logLevel,
            IncludeTransitiveDependencies = includeTransitive,
            SuppressPrintDetails = suppressPrintDetails,
            SuppressPrintIssues = suppressPrintIssues
        };

        return settings;
    }
}
