using Liz.Core.License.Contracts;

// ReSharper disable StringLiteralTypo

namespace Liz.Core.License.Sources.LicenseType;

internal sealed class EuplLicenseTypeDefinitionProvider : ILicenseTypeDefinitionProvider
{
    public IEnumerable<LicenseTypeDefinition> Get()
    {
        return new[]
        {
            new LicenseTypeDefinition("EUPL-1.0", "European Union Public Licence", "V.1.0"),
            new LicenseTypeDefinition("EUPL-1.1", "European Union Public Licence", "V.1.1"),
            new LicenseTypeDefinition("EUPL-1.2", "European Union Public Licence", "v. 1.2")
        };
    }
}
