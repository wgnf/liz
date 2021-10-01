using DotnetNugetLicenses.Core.Contracts;
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
        }

        public static void RegisterOtherServices(this IUnityContainer container)
        {
            container.RegisterSingleton<IFileSystem, FileSystem>();
        }
    }
}
