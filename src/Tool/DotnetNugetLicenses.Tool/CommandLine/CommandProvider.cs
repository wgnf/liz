using DotnetNugetLicenses.Core.Logging;
using DotnetNugetLicenses.Tool.Contracts.CommandLine;
using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.IO;

namespace DotnetNugetLicenses.Tool.CommandLine;

internal sealed class CommandProvider : ICommandProvider
{
    private readonly ICommandRunner _commandRunner;

    public CommandProvider(ICommandRunner commandRunner = null)
    {
        _commandRunner = commandRunner ?? new CommandRunner();
    }

    public RootCommand Get()
    {
        var rootCommand = new RootCommand("dotnet-tool to analyze the licenses of your project(s)");

        var options = GetOptions();
        foreach (var option in options) rootCommand.AddOption(option);

        rootCommand.Handler = CommandHandler.Create<FileInfo, LogLevel, bool>(_commandRunner.RunAsync);

        return rootCommand;
    }

    private static IEnumerable<Option> GetOptions()
    {
        var options = new List<Option>
        {
            GetTargetFileOption(), 
            GetLogLevelOption(),
            GetIncludeTransitiveOption()
        };
        return options;
    }

    private static Option GetTargetFileOption()
    {
        var option = new Option<FileInfo>(
            "--target",
            "The input file to analyze. Can be a Solution (sln) or a Project (csproj, fsproj)")
        {
            IsRequired = true,
            /*
             * NOTE: This has to match the parameter that is called (see CommandRunner)
             * or else CommandHandler does not know where to put the values
             */
            Name = "targetFile"
        };

        option.AddAlias("-t");
        option.AddSuggestions("./path/to/Solution.sln", "./path/to/Project.csproj");

        return option;
    }

    private static Option GetLogLevelOption()
    {
        var option = new Option<LogLevel>(
            "--log-level",
            () => LogLevel.Information,
            "The Log-Level that describes which messages are displayed when running the tool")
        {
            IsRequired = false, 
            Name = "logLevel"
        };

        option.AddAlias("-l");
        option.AddSuggestions(LogLevel.Information.ToString(), LogLevel.Error.ToString());
        return option;
    }

    private static Option GetIncludeTransitiveOption()
    {
        var option = new Option<bool>(
            "--include-transitive",
            () => false,
            "If transitive dependencies should be included or not")
        {
            IsRequired = false, 
            Name = "includeTransitive"
        };
        
        option.AddAlias("-i");
        return option;
    }
}
