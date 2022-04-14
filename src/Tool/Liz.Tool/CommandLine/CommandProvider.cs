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
            FileInfo? licenseTypeDefinitions,
            FileInfo? urlToLicenseTypeMapping) =>
        {
            await _commandRunner.RunAsync(
                targetFile, 
                logLevel, 
                includeTransitive, 
                suppressPrintDetails,
                suppressPrintIssues,
                suppressProgressbar,
                licenseTypeDefinitions,
                urlToLicenseTypeMapping)
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
            GetUrlToLicenseTypeMappingOption()
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
        var option = new Option<FileInfo?>(
            new[] { "--license-type-definitions", "-td" },
            "Provide a path to a JSON-File providing license-type-definitions which describe license-types by providing inclusive/exclusive license-text snippets");
        return option;
    }

    private static Option GetUrlToLicenseTypeMappingOption()
    {
        var option = new Option<FileInfo?>(
            new[] { "--url-type-mapping", "-um" },
            "Provide a path to a JSON-file (local or remote - remote will be downloaded automatically if available) containing a mapping from license-url (key) to license-type (value) for licenses whose license-type could not be determined");
        return option;
    }
}
