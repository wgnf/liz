using Liz.Core.License.Contracts;
using Liz.Core.License.Contracts.Models;

// ReSharper disable CommentTypo
// ReSharper disable StringLiteralTypo

namespace Liz.Core.License.Sources.LicenseType;

internal sealed class GnuLicenseTypeDefinitionProvider : ILicenseTypeDefinitionProvider
{
    public IEnumerable<LicenseTypeDefinition> Get()
    {
        return new[]
        {
            new LicenseTypeDefinition("GPL-1.0", "GNU GENERAL PUBLIC LICENSE", "Version 1")
            {
                ExclusiveTextSnippets = new []
                {
                    // this is part of MPL, which lists some secondary licenses...
                    "Secondary License",
                    // this is part of the LGPL
                    "Lesser",
                    // this is part of the AGPL
                    "Affero"
                }
            },
            new LicenseTypeDefinition("GPL-2.0", "GNU GENERAL PUBLIC LICENSE", "Version 2")
            {
                ExclusiveTextSnippets = new []
                {
                    // this is part of MPL, which lists some secondary licenses...
                    "Secondary License",
                    // this is part of the LGPL
                    "Lesser",
                    // this is part of the AGPL
                    "Affero"
                }
            },
            new LicenseTypeDefinition("GPL-3.0", "GNU GENERAL PUBLIC LICENSE", "Version 3")
            {
                ExclusiveTextSnippets = new []
                {
                    // this is part of MPL, which lists some secondary licenses...
                    "Secondary License",
                    // this is part of the LGPL
                    "Lesser",
                    // this is part of the AGPL
                    "Affero"
                }
            },
            new LicenseTypeDefinition("AGPL-1.0", "AFFERO GENERAL PUBLIC LICENSE", "Version 1")
            {
                // this is part of MPL, which lists some secondary licenses...
                ExclusiveTextSnippets = new []{ "Secondary License" }
            },
            new LicenseTypeDefinition("AGPL-3.0", "GNU AFFERO GENERAL PUBLIC LICENSE", "Version 3")
            {
                // this is part of MPL, which lists some secondary licenses...
                ExclusiveTextSnippets = new []{ "Secondary License" }
            },
            
            new LicenseTypeDefinition("LGPL-2.0", "GNU LIBRARY GENERAL PUBLIC LICENSE ", "Version 2,")
            {
                // this is part of MPL, which lists some secondary licenses...
                ExclusiveTextSnippets = new []{ "Secondary License" }
            },
            new LicenseTypeDefinition("LGPL-2.1", "GNU LESSER GENERAL PUBLIC LICENSE", "Version 2.1,")
            {
                // this is part of MPL, which lists some secondary licenses...
                ExclusiveTextSnippets = new []{ "Secondary License" }
            },
            new LicenseTypeDefinition("LGPL-3.0", "GNU LESSER GENERAL PUBLIC LICENSE", "Version 3,")
            {
                // this is part of MPL, which lists some secondary licenses...
                ExclusiveTextSnippets = new []{ "Secondary License" }
            }
        };
    }
}
