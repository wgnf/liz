using DotnetNugetLicenses.Tool.CommandLine;
using DotnetNugetLicenses.Tool.Contracts.CommandLine;
using Unity;

namespace DotnetNugetLicenses.Tool
{
    public static class ToolRegistrationExtensions
    {
        public static void RegisterToolServices(this IUnityContainer container)
        {
            container.RegisterType<ICommandProvider, CommandProvider>();
            container.RegisterType<ICommandRunner, CommandRunner>();
        }
    }
}
