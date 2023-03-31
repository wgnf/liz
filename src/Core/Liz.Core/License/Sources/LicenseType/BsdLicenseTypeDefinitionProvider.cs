using Liz.Core.License.Contracts;
using Liz.Core.License.Contracts.Models;

// ReSharper disable StringLiteralTypo

namespace Liz.Core.License.Sources.LicenseType;

internal sealed class BsdLicenseTypeDefinitionProvider : ILicenseTypeDefinitionProvider
{
    public IEnumerable<LicenseTypeDefinition> Get()
    {
        return new[]
        {
            new LicenseTypeDefinition("BSD-2-Clause", "BSD 2-Clause License"),
            new LicenseTypeDefinition("BSD-2-Clause", "1.", "2.",
                "Redistribution and use in source and binary forms, with or without modification, are permitted provided that the following conditions are met")
            {
                ExclusiveTextSnippets = new[]
                {
                    "3.", "Neither the name of the copyright holder nor the names of its contributors may be used"
                }
            },
            new LicenseTypeDefinition("BSD-3-Clause", "BSD 3-Clause License"),
            new LicenseTypeDefinition("BSD-3-Clause", "1.", "2.", "3.",
                "Redistribution and use in source and binary forms, with or without modification, are permitted provided that the following conditions are met",
                "Neither the name of the copyright holder nor the names of its contributors may be used")
        };
    }
}
