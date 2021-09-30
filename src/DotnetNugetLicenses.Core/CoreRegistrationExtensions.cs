using DotnetNugetLicenses.Core.Contracts;
using System.IO.Abstractions;
using Unity;

namespace DotnetNugetLicenses.Core
{
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
