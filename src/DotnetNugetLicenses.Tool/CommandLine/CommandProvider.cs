using DotnetNugetLicenses.Tool.Contracts.CommandLine;
using System.Collections.Generic;
using System.CommandLine;
using System.IO;

namespace DotnetNugetLicenses.Tool.CommandLine
{
    public class CommandProvider : ICommandProvider
    {
        public RootCommand Get()
        {
            var rootCommand = new RootCommand("dotnet-tool to analyze the licenses of your project(s)");

            var options = GetOptions();
            foreach (var option in options) rootCommand.AddOption(option);

            return rootCommand;
        }

        private static IEnumerable<Option> GetOptions()
        {
            var options = new List<Option>
            {
                GetTargetFileOption()
            };
            return options;
        }

        private static Option GetTargetFileOption()
        {
            var option = new Option<FileInfo>(
                "--target",
                description: "The input file to analyze. Can be a Solution (sln) or a Project (csproj, fsproj)")
            {
                IsRequired = true,
                Name = "target"
            };

            option.AddAlias("-t");
            option.AddSuggestions("./path/to/your/Solution.sln", "./path/to/your/Project.csproj");

            return option;
        }
    }
}
