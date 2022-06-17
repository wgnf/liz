using Liz.Core.License.Contracts;
using Liz.Core.License.Contracts.Models;

// ReSharper disable StringLiteralTypo

namespace Liz.Core.License.Sources.LicenseType;

internal sealed class CddlLicenseTypeDefinitionProvider : ILicenseTypeDefinitionProvider
{
    public IEnumerable<LicenseTypeDefinition> Get()
    {
        return new[]
        {
            new LicenseTypeDefinition("CDDL-1.0", "COMMON DEVELOPMENT AND DISTRIBUTION LICENSE", "Version 1.0"),
            new LicenseTypeDefinition("CDDL-1.1", "COMMON DEVELOPMENT AND DISTRIBUTION LICENSE", "Version 1.1")
        };
    }
}
