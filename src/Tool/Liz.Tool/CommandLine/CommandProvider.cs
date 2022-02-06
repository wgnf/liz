using Liz.Core.Logging.Contracts;
using Liz.Tool.Contracts.CommandLine;
using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.Binding;
using System.IO;
using System.Linq;

namespace Liz.Tool.CommandLine;

internal sealed class CommandProvider
{
    private readonly ICommandRunner _commandRunner;

    public CommandProvider(ICommandRunner commandRunner = null)
    {
        _commandRunner = commandRunner ?? new CommandRunner();
    }

    public RootCommand Get()
    {
        var rootCommand = new RootCommand("dotnet-tool to analyze the licenses of your project(s)");
        var symbols = new List<IValueDescriptor>();

        var targetFileArgument = GetTargetFileArgument();
        rootCommand.AddArgument(targetFileArgument);
        symbols.Add(targetFileArgument);

        var options = GetOptions().ToList();
        foreach (var option in options) rootCommand.AddOption(option);
        symbols.AddRange(options);

        rootCommand.SetHandler( async (FileInfo targetFile, LogLevel logLevel, bool includeTransitive) =>
        {
            await _commandRunner.RunAsync(targetFile, logLevel, includeTransitive).ConfigureAwait(false);
        }, symbols.ToArray());

        return rootCommand;
    }

    private static IEnumerable<Option> GetOptions()
    {
        var options = new List<Option>
        {
            GetLogLevelOption(),
            GetIncludeTransitiveOption()
        };
        return options;
    }

    private static Argument GetTargetFileArgument()
    {
        var argument = new Argument<FileInfo>(
            "targetFile",
            "The input file to analyze. Can be a Solution (sln) or a Project (csproj, fsproj)");
        return argument;
    }

    private static Option GetLogLevelOption()
    {
        var option = new Option<LogLevel>(
            new[] { "--log-level", "-l" },
            () => LogLevel.Information,
            "The Log-Level that describes which messages are displayed when running the tool");
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
}
