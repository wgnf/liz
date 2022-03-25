using Liz.Core.License.Contracts;

// ReSharper disable StringLiteralTypo

namespace Liz.Core.License.Sources.LicenseType;

internal sealed class LicenseTypeDefinitionProvider : ILicenseTypeDefinitionProvider
{
    public IEnumerable<LicenseTypeDefinition> Get()
    {
        var defaultDefinitions = GetDefaultDefinitions();
        return defaultDefinitions;
    }

    private static IEnumerable<LicenseTypeDefinition> GetDefaultDefinitions()
    {
        return new[]
        {
            new LicenseTypeDefinition("MIT", "MIT License")
        };
    }
}
