using DotnetNugetLicenses.Core.Contracts;
using DotnetNugetLicenses.Core.Contracts.Services;
using DotnetNugetLicenses.Core.Services;
using SlnParser;
using SlnParser.Contracts;
using System.Diagnostics.CodeAnalysis;
using System.IO.Abstractions;
using Unity;

namespace DotnetNugetLicenses.Core
{
    [ExcludeFromCodeCoverage] // do not want to test registrations
    public static class CoreRegistrationExtensions
    {
        public static void RegisterCoreServices(this IUnityContainer container)
        {
            container.RegisterType<IExtractLicenses, ExtractLicenses>();
            container.RegisterType<IGetProjects, DefaultGetProjects>();
        }

        public static void RegisterOtherServices(this IUnityContainer container)
        {
            container.RegisterSingleton<IFileSystem, FileSystem>();
            container.RegisterSingleton<ISolutionParser, SolutionParser>();
        }
    }
}
