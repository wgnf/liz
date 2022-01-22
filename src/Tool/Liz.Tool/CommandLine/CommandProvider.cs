using Liz.Core.Logging.Contracts;
using Liz.Tool.Contracts.CommandLine;
using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.IO;

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

        var targetFileArgument = GetTargetFileArgument();
        rootCommand.AddArgument(targetFileArgument);
            
        var options = GetOptions();
        foreach (var option in options) rootCommand.AddOption(option);

        rootCommand.Handler = CommandHandler.Create<FileInfo, LogLevel, bool>(_commandRunner.RunAsync);

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

        option.AddSuggestions(LogLevel.Information.ToString(), LogLevel.Error.ToString());
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
