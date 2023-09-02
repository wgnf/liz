using Liz.Core.Logging.Contracts;

namespace Liz.Tool.Contracts.CommandLine;

public interface ICommandRunner
{
    Task RunAsync(
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
        string? exportTexts,
        string? exportJson,
        int? requestTimeout);
}
