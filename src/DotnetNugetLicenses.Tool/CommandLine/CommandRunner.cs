using Ardalis.GuardClauses;
using DotnetNugetLicenses.Core.Contracts;
using DotnetNugetLicenses.Tool.Contracts.CommandLine;
using Microsoft.Extensions.Logging;
using System.IO;
using System.IO.Abstractions;
using Unity;

namespace DotnetNugetLicenses.Tool.CommandLine
{
	public sealed class CommandRunner : ICommandRunner
	{
        private readonly IExtractLicenses _extractLicenses;
		private readonly IFileSystem _fileSystem;
        private readonly IUnityContainer _unityContainer;

        public CommandRunner(
            IExtractLicenses extractLicenses, 
            IFileSystem fileSystem,
            IUnityContainer unityContainer)
		{
            _extractLicenses = Guard.Against.Null(extractLicenses, nameof(extractLicenses));
			_fileSystem = Guard.Against.Null(fileSystem, nameof(fileSystem));
            _unityContainer = Guard.Against.Null(unityContainer, nameof(unityContainer));
        }

		public void Run(FileInfo targetFile, LogLevel logLevel)
		{
			Guard.Against.Null(targetFile, nameof(targetFile));

            // NOTE: We have to do that here, because this is the place where we parsed the log-level
            _unityContainer.RegisterLoggingServices(logLevel);

            var settings = CreateSettings(targetFile);
			_extractLicenses.Extract(settings);
		}

		private ExtractSettings CreateSettings(FileInfo targetFile)
		{
			var targetFileInfo = new FileInfoWrapper(_fileSystem, targetFile);

			var settings = new ExtractSettings(targetFileInfo);

			return settings;
		}
	}
}
