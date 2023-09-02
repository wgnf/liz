using System.CommandLine;
using System.Diagnostics.CodeAnalysis;
using Liz.Core;
using Liz.Core.Logging.Contracts;
using Liz.Core.Progress;
using Liz.Tool.Logging;
using Liz.Tool.Progress;

namespace Liz.Tool;

[ExcludeFromCodeCoverage] // mostly untestable startup code
public static class Program
{
    /*
     * NOTE:
     * we unfortunately have to do it this way now, because we exceeded the maximum amount of parameters for
     * 'SetHandler()' when doing it individually.
     *
     * The tests were pretty silly anyways.
     *
     * But this approach is kinda ugly too. As we can't really break anything up into methods as we need the local variables
     * everywhere...
     */
    public static async Task<int> Main(string[] args)
    {
        var rootCommand = new RootCommand("liz - a dotnet-tool to analyze the licenses of your project(s)");
        
        var targetFileArgument = new Argument<FileInfo>(
            "targetFile",
            "The target file to analyze. Can be a solution (.sln) or a project (.csproj, .fsproj) file");
        rootCommand.AddArgument(targetFileArgument);

        var logLevelOption = new Option<LogLevel>(
            new[] { "--log-level", "-l" },
            () => LogLevel.Information,
            "The log-level which describes what kind of messages are displayed when running the tool");
        rootCommand.AddOption(logLevelOption);

        var includeTransitiveOption = new Option<bool>(
            new[] { "--include-transitive", "-i" },
            () => false,
            "If transitive dependencies should be included or not");
        rootCommand.AddOption(includeTransitiveOption);

        var suppressPrintDetailsOption = new Option<bool>(
            new[] { "--suppress-print-details", "-sd" },
            () => false,
            "If printing the license and package-reference details should be suppressed or not");
        rootCommand.AddOption(suppressPrintDetailsOption);

        var suppressPrintIssuesOption = new Option<bool>(
            new[] { "--suppress-print-issues", "-si" },
            () => false,
            "If printing the license-information issues should be suppressed or not");
        rootCommand.AddOption(suppressPrintIssuesOption);
        
        var suppressProgressbarOption = new Option<bool>(
            new[] { "--suppress-progressbar", "-sb" },
            () => false,
            "If displaying the progressbar should be suppressed or not. Can help when debugging errors or is used in a CI/CD Pipeline");
        rootCommand.AddOption(suppressProgressbarOption);
        
        var licenseTypeDefinitionsOption = new Option<string?>(
            new[] { "--license-type-definitions", "-td" },
            () => null,
            "Provide a path to a JSON-File (local or remote - remote will be downloaded automatically if available) providing license-type-definitions which describe license-types by providing inclusive/exclusive license-text snippets");
        rootCommand.AddOption(licenseTypeDefinitionsOption);
        
        var urlTypeMappingOption = new Option<string?>(
            new[] { "--url-type-mapping", "-um" },
            () => null,
            "Provide a path to a JSON-file (local or remote - remote will be downloaded automatically if available) containing a mapping from license-url (key) to license-type (value) for licenses whose license-type could not be determined");
        rootCommand.AddOption(urlTypeMappingOption);
        
        var whitelistOption = new Option<string?>(
            new[] { "--whitelist", "-w" },
            () => null,
            "Provide a path to a JSON-File (local or remote - remote will be downloaded automatically if available) containing a list of license-types, which are the only ones allowed, when validating the determined license-types. Any license-type which is not in the whitelist will cause the validation to fail. '--whitelist' and '--blacklist' are mutually exclusive!");
        rootCommand.AddOption(whitelistOption);
        
        var blacklistOption = new Option<string?>(
            new[] { "--blacklist", "-b" },
            () => null,
            "Provide a path to a JSON-File (local or remote - remote will be downloaded automatically if available) containing a list of license-types, which are the only ones disallowed, when validating the determined license-types. Any license-type that is the same as within that blacklist will cause the validation to fail. Any other license-type is allowed. '--whitelist' and '--blacklist' are mutually exclusive!");
        rootCommand.AddOption(blacklistOption);
        
        var exportTextsOption = new Option<string?>(
            new[] { "--export-texts", "-et" },
            () => null,
            "A path to a directory to where the determined license-texts will be exported. Each license-text will be written to an individual file with the file-name being: \"<package-name>-<package-version>.txt\". If the license-text is the content of a website, the contents will be written into an \".html\" file instead");
        rootCommand.AddOption(exportTextsOption);
        
        var exportJsonOption = new Option<string?>(
            new[] { "--export-json", "-ej" },
            () => null,
            "A path to a JSON-file t which the determined license- and package-information will be exported. All the information will be written to a single JSON-file. If the file already exists it will be overwritten.");
        rootCommand.AddOption(exportJsonOption);
        
        var timeoutOption = new Option<int?>(
            new[] { "--timeout", "-t" },
            () => null,
            "The timeout for a request (i.e. to get the license text from a website). After this amount of time a request will be considered as failed and aborted.");
        rootCommand.AddOption(timeoutOption);
        
        var projectExclusionsOption = new Option<string?>(
            new[] { "--project-excludes", "-pe" },
            () => null,
            "A path to a JSON-File (local or remote - remote will be downloaded automatically if available) containing a list of glob-patterns to exclude certain projects. A project will be excluded when it matches at least one glob-pattern. The pattern will be matched against the absolute path of the project-file. All available patterns can be found here: https://github.com/dazinator/DotNet.Glob/tree/3.1.3#patterns");
        rootCommand.AddOption(projectExclusionsOption);
        
        var packageExclusionsOption = new Option<string?>(
            new[] { "--dependency-excludes", "-de" },
            () => null,
            "A path to a JSON-File (local or remote - remote will be downloaded automatically if available) containing a list of glob-patterns to exclude certain packages. A package will be excluded when it matches at least one glob-pattern. The pattern will be matched against the name of the package. All available patterns can be found here: https://github.com/dazinator/DotNet.Glob/tree/3.1.3#patterns");
        rootCommand.AddOption(packageExclusionsOption);
        
        rootCommand.SetHandler(async context =>
        {
            var targetFile = context.ParseResult.GetValueForArgument(targetFileArgument);

            var settings = new ExtractLicensesSettings(targetFile.FullName)
            {
                IncludeTransitiveDependencies = context.ParseResult.GetValueForOption(includeTransitiveOption),
                SuppressPrintDetails = context.ParseResult.GetValueForOption(suppressPrintDetailsOption),
                SuppressPrintIssues = context.ParseResult.GetValueForOption(suppressPrintIssuesOption),
                LicenseTypeDefinitionsFilePath = context.ParseResult.GetValueForOption(licenseTypeDefinitionsOption),
                UrlToLicenseTypeMappingFilePath = context.ParseResult.GetValueForOption(urlTypeMappingOption),
                LicenseTypeWhitelistFilePath = context.ParseResult.GetValueForOption(whitelistOption),
                LicenseTypeBlacklistFilePath = context.ParseResult.GetValueForOption(blacklistOption),
                ExportLicenseTextsDirectory = context.ParseResult.GetValueForOption(exportTextsOption),
                ExportJsonFile = context.ParseResult.GetValueForOption(exportJsonOption),
                ProjectExclusionGlobsFilePath = context.ParseResult.GetValueForOption(projectExclusionsOption),
                PackageExclusionGlobsFilePath = context.ParseResult.GetValueForOption(packageExclusionsOption)
            };

            var requestTimeout = context.ParseResult.GetValueForOption(timeoutOption);
            if (requestTimeout != null)
            {
                settings.RequestTimeout = TimeSpan.FromSeconds(requestTimeout.Value);
            }
            
            ILoggerProvider? loggerProvider;
            IProgressHandler? progressHandler;

            var commandLineLoggerProvider = new CommandLineLoggerProvider();
            var suppressProgressbar = context.ParseResult.GetValueForOption(suppressProgressbarOption);
            var logLevel = context.ParseResult.GetValueForOption(logLevelOption);
        
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

            var extractLicensesFactory = new ExtractLicensesFactory();
            var extractLicenses = extractLicensesFactory.Create(settings, loggerProvider, logLevel, progressHandler);
            await extractLicenses.ExtractAsync().ConfigureAwait(false);
        });

        var exitCode = await rootCommand.InvokeAsync(args);
        return exitCode;
    }
}
