using Ardalis.GuardClauses;
using DotnetNugetLicenses.Core.Contracts;
using DotnetNugetLicenses.Tool.Contracts.CommandLine;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.IO.Abstractions;
using Unity;

namespace DotnetNugetLicenses.Tool.CommandLine
{
	public sealed class CommandRunner : ICommandRunner
	{
        private readonly Func<IExtractLicenses> _extractLicensesFactory;
		private readonly IFileSystem _fileSystem;
        private readonly IUnityContainer _unityContainer;

        public CommandRunner(
            /*
             * NOTE
             * We need to use a Func-Factory here, because we only register the Logging-Stuff as soon as we
             * call the Run()-Method of this CommandRunner.
             * But because this CommandRunner depends on other services that might depend on the ILogger<>
             * that leads to problems when Unity tries to resolve the instance of this CommandRunner
             * (because the ILogger<> is not registered yet)
             */
            Func<IExtractLicenses> extractLicensesFactory, 
            IFileSystem fileSystem,
            IUnityContainer unityContainer)
		{
            _extractLicensesFactory = Guard.Against.Null(extractLicensesFactory, nameof(extractLicensesFactory));
			_fileSystem = Guard.Against.Null(fileSystem, nameof(fileSystem));
            _unityContainer = Guard.Against.Null(unityContainer, nameof(unityContainer));
        }

		public void Run(FileInfo targetFile, LogLevel logLevel)
		{
			Guard.Against.Null(targetFile, nameof(targetFile));

            // NOTE: We have to do that here, because this is the place where we parsed the log-level
            _unityContainer.RegisterLoggingServices(logLevel);

            var settings = CreateSettings(targetFile);
			_extractLicensesFactory().Extract(settings);
		}

		private ExtractSettings CreateSettings(FileInfo targetFile)
		{
			var targetFileInfo = new FileInfoWrapper(_fileSystem, targetFile);

			var settings = new ExtractSettings(targetFileInfo);

			return settings;
		}
	}
}
