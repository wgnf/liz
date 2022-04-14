using Liz.Core.License.Contracts;
using Liz.Core.License.Contracts.Models;

// ReSharper disable StringLiteralTypo

namespace Liz.Core.License.Sources.LicenseType;

internal sealed class MplLicenseTypeDefinitionProvider : ILicenseTypeDefinitionProvider
{
    public IEnumerable<LicenseTypeDefinition> Get()
    {
        return new[]
        {
            new LicenseTypeDefinition("MPL-1.0", "MOZILLA PUBLIC LICENSE Version 1.0"),
            new LicenseTypeDefinition("MPL-1.0", "MOZILLA PUBLIC LICENSE 1.0"),
            new LicenseTypeDefinition("MPL-1.1", "Mozilla Public License Version 1.1"),
            new LicenseTypeDefinition("MPL-1.1", "Mozilla Public License 1.1"),
            new LicenseTypeDefinition("MPL-2.0", "Mozilla Public License Version 2.0"),
            new LicenseTypeDefinition("MPL-2.0", "Mozilla Public License 2.0")
        };
    }
}
