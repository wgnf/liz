using Ardalis.GuardClauses;
using DotnetNugetLicenses.Core.Contracts;
using DotnetNugetLicenses.Tool.Contracts.CommandLine;
using System.IO.Abstractions;

namespace DotnetNugetLicenses.Tool.CommandLine
{
    public class CommandRunner : ICommandRunner
    {
		private readonly IExtractLicenses _extractLicenses;
		private readonly IFileSystem _fileSystem;

		public CommandRunner(IExtractLicenses extractLicenses, IFileSystem fileSystem)
		{
			_extractLicenses = Guard.Against.Null(extractLicenses, nameof(extractLicenses));
			_fileSystem = Guard.Against.Null(fileSystem, nameof(fileSystem));
		}

        public void Run(string targetFile)
        {
            Guard.Against.Null(targetFile, nameof(targetFile));

			var settings = CreateSettings(targetFile);
			_extractLicenses.Extract(settings);
        }

		private ExtractSettings CreateSettings(string targetFile)
		{
			var targetFileInfo = _fileSystem.FileInfo.FromFileName(targetFile);

			var settings = new ExtractSettings(targetFileInfo);

			return settings;
		}
    }
}
