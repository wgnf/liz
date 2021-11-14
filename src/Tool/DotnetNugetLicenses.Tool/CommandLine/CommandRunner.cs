using DotnetNugetLicenses.Core;
using DotnetNugetLicenses.Core.Logging;
using DotnetNugetLicenses.Core.Settings;
using DotnetNugetLicenses.Tool.Contracts.CommandLine;
using DotnetNugetLicenses.Tool.Logging;
using System;
using System.IO;

namespace DotnetNugetLicenses.Tool.CommandLine;

internal sealed class CommandRunner : ICommandRunner
{
    private readonly IExtractLicensesFactory _extractLicensesFactory;
    
    public CommandRunner(IExtractLicensesFactory extractLicensesFactory = null)
    {
        _extractLicensesFactory = extractLicensesFactory ?? new ExtractLicensesFactory();
    }
    
    public void Run(FileInfo targetFile, LogLevel logLevel)
    {
        if (targetFile == null) throw new ArgumentNullException(nameof(targetFile));

        var settings = CreateSettings(targetFile, logLevel);
        var loggerProvider = new CommandLineLoggerProvider();
        
        var extractLicenses = _extractLicensesFactory.Create(settings, loggerProvider);
        extractLicenses.Extract();
    }

    private static ExtractLicensesSettings CreateSettings(
        FileSystemInfo targetFile,
        LogLevel logLevel)
    {
        var settings = new ExtractLicensesSettings(targetFile.FullName) { LogLevel = logLevel };

        return settings;
    }
}
