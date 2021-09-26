using DotnetNugetLicenses.Core;
using DotnetNugetLicenses.Core.Contracts;
using DotnetNugetLicenses.Tool.CommandLine;
using DotnetNugetLicenses.Tool.Contracts.CommandLine;
using System.CommandLine;
using System.Diagnostics.CodeAnalysis;
using System.IO.Abstractions;
using System.Threading.Tasks;
using Unity;

namespace DotnetNugetLicenses.Tool
{
	[ExcludeFromCodeCoverage] // mostly untestable
	public static class Program
	{
		public static Task<int> Main(string[] args)
		{
			using var container = InitializeContainer();

			var commandProvider = container.Resolve<ICommandProvider>();
			var rootCommand = commandProvider.Get();

			return rootCommand.InvokeAsync(args);
		}

		private static IUnityContainer InitializeContainer()
		{
			var container = new UnityContainer();

			RegisterTypes(container);

			return container;
		}

		private static void RegisterTypes(IUnityContainer container)
		{
			RegisterCommandLineServices(container);
			RegisterCoreServices(container);

			RegisterOtherServices(container);
		}

		private static void RegisterCommandLineServices(IUnityContainer container)
		{
			container.RegisterType<ICommandProvider, CommandProvider>();
			container.RegisterType<ICommandRunner, CommandRunner>();
		}

		private static void RegisterCoreServices(IUnityContainer container)
		{
			container.RegisterType<IExtractLicenses, ExtractLicenses>();
		}

		private static void RegisterOtherServices(IUnityContainer container)
		{
			container.RegisterSingleton<IFileSystem, FileSystem>();
		}
	}
}