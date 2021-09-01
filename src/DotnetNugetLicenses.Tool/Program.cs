using DotnetNugetLicenses.Tool.CommandLine;
using DotnetNugetLicenses.Tool.Contracts.CommandLine;
using System.CommandLine;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Unity;

namespace DotnetNugetLicenses
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
            container.RegisterType<ICommandProvider, CommandProvider>();
        }
    }
}
