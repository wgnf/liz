using Liz.Core.License.Contracts;
using Liz.Core.License.Contracts.Models;

// ReSharper disable StringLiteralTypo

namespace Liz.Core.License.Sources.LicenseType;

internal sealed class MicrosoftLicenseTypeDefinitionProvider : ILicenseTypeDefinitionProvider
{
    public IEnumerable<LicenseTypeDefinition> Get()
    {
        return new[]
        {
            new LicenseTypeDefinition("MS-PL", "Microsoft Public License"),
            new LicenseTypeDefinition("MS-NETLIB", "MICROSOFT .NET LIBRARY")
        };
    }
}
