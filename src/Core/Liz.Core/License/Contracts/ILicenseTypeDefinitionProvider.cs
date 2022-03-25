using Liz.Core.License.Sources.LicenseType;

namespace Liz.Core.License.Contracts;

internal interface ILicenseTypeDefinitionProvider
{
    IEnumerable<LicenseTypeDefinition> Get();
}
