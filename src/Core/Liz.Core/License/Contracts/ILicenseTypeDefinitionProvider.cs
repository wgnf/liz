using Liz.Core.License.Contracts.Models;

namespace Liz.Core.License.Contracts;

internal interface ILicenseTypeDefinitionProvider
{
    IEnumerable<LicenseTypeDefinition> Get();
}
