using Liz.Core.License.Contracts;
using Liz.Core.License.Contracts.Models;

// ReSharper disable StringLiteralTypo

namespace Liz.Core.License.Sources.LicenseType;

internal sealed class NplLicenseTypeDefinitionProvider : ILicenseTypeDefinitionProvider
{
    public IEnumerable<LicenseTypeDefinition> Get()
    {
        return new[]
        {
            new LicenseTypeDefinition("NPL-1.0", "NETSCAPE PUBLIC LICENSE", "Version 1.0"),
            new LicenseTypeDefinition("NPL-1.1", "Netscape Public LIcense", "version 1.1")
        };
    }
}
