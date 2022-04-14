using Liz.Core.License.Contracts;
using Liz.Core.License.Contracts.Models;

// ReSharper disable StringLiteralTypo

namespace Liz.Core.License.Sources.LicenseType;

internal sealed class EplLicenseTypeDefinitionProvider : ILicenseTypeDefinitionProvider
{
    public IEnumerable<LicenseTypeDefinition> Get()
    {
        return new[]
        {
            new LicenseTypeDefinition("EPL-1.0", "Eclipse Public License", "v 1.0"),
            new LicenseTypeDefinition("EPL-2.0", "Eclipse Public License", "v 2.0")
        };
    }
}
