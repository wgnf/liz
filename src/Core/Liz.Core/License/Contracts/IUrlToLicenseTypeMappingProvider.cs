namespace Liz.Core.License.Contracts;

internal interface IUrlToLicenseTypeMappingProvider
{
    IDictionary<string, string> Get();
}
