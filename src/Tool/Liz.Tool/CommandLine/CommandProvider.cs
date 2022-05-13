using Liz.Core.Logging.Contracts;
using Liz.Tool.Contracts.CommandLine;
using System.CommandLine;
using System.CommandLine.Binding;
using System.Diagnostics.CodeAnalysis;

namespace Liz.Tool.CommandLine;

internal sealed class CommandProvider
{
    private readonly ICommandRunner _commandRunner;

    public CommandProvider(ICommandRunner? commandRunner = null)
    {
        _commandRunner = commandRunner ?? new CommandRunner();
    }

    [ExcludeFromCodeCoverage] // running root-command cannot easily be tested
    public RootCommand Get()
    {
        var (rootCommand, symbols) = PrepareRootCommand();

        rootCommand.SetHandler(async (
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
            string? exportTexts) =>
        {
            await _commandRunner.RunAsync(
                targetFile, 
                logLevel, 
                includeTransitive, 
                suppressPrintDetails,
                suppressPrintIssues,
                suppressProgressbar,
                licenseTypeDefinitions,
                urlToLicenseTypeMapping,
                whitelist,
                blacklist,
                exportTexts)
                .ConfigureAwait(false);
        }, symbols.ToArray());

        return rootCommand;
    }

    private static (RootCommand rootCommand, List<IValueDescriptor> symbols) PrepareRootCommand()
    {
        var rootCommand = new RootCommand("dotnet-tool to analyze the licenses of your project(s)");
        var symbols = new List<IValueDescriptor>();

        var targetFileArgument = GetTargetFileArgument();
        rootCommand.AddArgument(targetFileArgument);
        symbols.Add(targetFileArgument);

        var options = GetOptions().ToList();
        foreach (var option in options) rootCommand.AddOption(option);
        symbols.AddRange(options);
        return (rootCommand, symbols);
    }

    private static IEnumerable<Option> GetOptions()
    {
        var options = new List<Option>
        {
            GetLogLevelOption(),
            GetIncludeTransitiveOption(),
            GetSuppressPrintDetailsOption(),
            GetSuppressPrintIssuesOption(),
            GetSuppressProgressBarOption(),
            GetLicenseTypeDefinitionsOption(),
            GetUrlToLicenseTypeMappingOption(),
            GetLicenseTypeWhitelistOption(),
            GetLicenseTypeBlacklistOption(),
            GetExportLicenseTextsDirectoryOption()
        };
        return options;
    }

    private static Argument GetTargetFileArgument()
    {
        var argument = new Argument<FileInfo>(
            "targetFile",
            "The target file to analyze. Can be a solution (.sln) or a project (.csproj, .fsproj) file");
        return argument;
    }

    private static Option GetLogLevelOption()
    {
        var option = new Option<LogLevel>(
            new[] { "--log-level", "-l" },
            () => LogLevel.Information,
            "The log-level which describes what kind of messages are displayed when running the tool");
        return option;
    }

    private static Option GetIncludeTransitiveOption()
    {
        var option = new Option<bool>(
            new[] { "--include-transitive", "-i" },
            () => false,
            "If transitive dependencies should be included or not");
        return option;
    }

    private static Option GetSuppressPrintDetailsOption()
    {
        var option = new Option<bool>(
            new[] { "--suppress-print-details", "-sd" },
            () => false,
            "If printing the license and package-reference details should be suppressed or not");
        return option;
    }

    private static Option GetSuppressPrintIssuesOption()
    {
        var option = new Option<bool>(
            new[] { "--suppress-print-issues", "-si" },
            () => false,
            "If printing the license-information issues should be suppressed or not");
        return option;
    }

    private static Option GetSuppressProgressBarOption()
    {
        var option = new Option<bool>(
            new[] { "--suppress-progressbar", "-sb" },
            () => false,
            "If displaying the progressbar should be suppressed or not. Can help when debugging errors or is used in a CI/CD Pipeline");
        return option;
    }

    private static Option GetLicenseTypeDefinitionsOption()
    {
        var option = new Option<string?>(
            new[] { "--license-type-definitions", "-td" },
            () => null,
            "Provide a path to a JSON-File (local or remote - remote will be downloaded automatically if available) providing license-type-definitions which describe license-types by providing inclusive/exclusive license-text snippets");
        return option;
    }

    private static Option GetUrlToLicenseTypeMappingOption()
    {
        var option = new Option<string?>(
            new[] { "--url-type-mapping", "-um" },
            () => null,
            "Provide a path to a JSON-file (local or remote - remote will be downloaded automatically if available) containing a mapping from license-url (key) to license-type (value) for licenses whose license-type could not be determined");
        return option;
    }

    private static Option GetLicenseTypeWhitelistOption()
    {
        var option = new Option<string?>(
            new[] { "--whitelist", "-w" },
            () => null,
            "Provide a path to a JSON-File (local or remote - remote will be downloaded automatically if available) containing a list of license-types, which are the only ones allowed, when validating the determined license-types. Any license-type which is not in the whitelist will cause the validation to fail. '--whitelist' and '--blacklist' are mutually exclusive!");
        return option;
    }

    private static Option GetLicenseTypeBlacklistOption()
    {
        var option = new Option<string?>(
            new[] { "--blacklist", "-b" },
            () => null,
            "Provide a path to a JSON-File (local or remote - remote will be downloaded automatically if available) containing a list of license-types, which are the only ones disallowed, when validating the determined license-types. Any license-type that is the same as within that blacklist will cause the validation to fail. Any other license-type is allowed. '--whitelist' and '--blacklist' are mutually exclusive!");
        return option;
    }

    private static Option GetExportLicenseTextsDirectoryOption()
    {
        var option = new Option<string?>(
            new[] { "--export-texts", "-et" },
            () => null,
            "A path to a directory to where the determined license-texts will be exported. Each license-text will be written to an individual file with the file-name being: \"<package-name>-<package-version>.txt\". If the license-text is the content of a website, the contents will be written into an \".html\" file instead");
        return option;
    }
}
