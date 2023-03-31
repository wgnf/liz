using Liz.Core.License.Contracts;
using Liz.Core.License.Contracts.Models;

// ReSharper disable StringLiteralTypo

namespace Liz.Core.License.Sources.LicenseType;

internal sealed class PopularLicensesLicenseTypeDefinitionProvider : ILicenseTypeDefinitionProvider
{
    // see https://spdx.org/licenses/ for a full list of SPDX identifiers (just added a subset of popular ones for now)
    public IEnumerable<LicenseTypeDefinition> Get()
    {
        return new[]
        {
            new LicenseTypeDefinition("MIT", "MIT License"), new LicenseTypeDefinition("ISC", "ISC License"),
            new LicenseTypeDefinition("Artistic-1.0", "The Artistic License"),
            new LicenseTypeDefinition("CPOL-1.02", "The Code Project Open License", "1.02"),
            new LicenseTypeDefinition("WTFPL", "DO WHAT THE FUCK YOU WANT TO PUBLIC LICENSE "),
            new LicenseTypeDefinition("Zlib", "zlib License"),
            new LicenseTypeDefinition("Unlicense", "unlicense.org",
                "This is free and unencumbered software released into the public domain."),
            new LicenseTypeDefinition("ANTLR-PD", "ANTLR", "2"),
            new LicenseTypeDefinition("BSL-1.0", "Boost Software License", "Version 1.0"),
            new LicenseTypeDefinition("ICU", "ICU License"), new LicenseTypeDefinition("Info-ZIP", "Info-ZIP License"),
            new LicenseTypeDefinition("CPAL-1.0", "Common Public Attribution License", "Version 1.0"),
            new LicenseTypeDefinition("CPL-1.0", "Common Public License", "Version 1.0"),
            new LicenseTypeDefinition("IPL-1.0", "IBM Public License", "Version 1.0")
        };
    }
}
