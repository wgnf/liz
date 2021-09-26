using Ardalis.GuardClauses;
using DotnetNugetLicenses.Tool.Contracts.CommandLine;
using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.IO;

namespace DotnetNugetLicenses.Tool.CommandLine
{
	public class CommandProvider : ICommandProvider
	{
		private readonly ICommandRunner _commandRunner;

		public CommandProvider(ICommandRunner commandRunner)
		{
			_commandRunner = Guard.Against.Null(commandRunner, nameof(commandRunner));
		}

		public RootCommand Get()
		{
			var rootCommand = new RootCommand("dotnet-tool to analyze the licenses of your project(s)");

			var options = GetOptions();
			foreach (var option in options) rootCommand.AddOption(option);

			rootCommand.Handler = CommandHandler.Create<FileInfo>(_commandRunner.Run);

			return rootCommand;
		}

		private static IEnumerable<Option> GetOptions()
		{
			var options = new List<Option> { GetTargetFileOption() };
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
	}
}