using Liz.Core.License.Contracts;

// ReSharper disable StringLiteralTypo

namespace Liz.Core.License.Sources.LicenseType;

internal sealed class PopularLicensesLicenseTypeDefinitionProvider : ILicenseTypeDefinitionProvider
{
    public IEnumerable<LicenseTypeDefinition> Get()
    {
        var defaultDefinitions = GetDefaultDefinitions();
        return defaultDefinitions;
    }

    // see https://spdx.org/licenses/ for a full list of SPDX identifiers (just added a subset of popular ones for now)
    private static IEnumerable<LicenseTypeDefinition> GetDefaultDefinitions()
    {
        return new[]
        {
            new LicenseTypeDefinition("MIT", "MIT License"),
            
            new LicenseTypeDefinition("GPL-1.0", "GNU GENERAL PUBLIC LICENSE", "Version 1")
            {
                // this is part of MPL, which lists some secondary licenses...
                ExclusionTextSnippets = new []{ "Secondary License" }
            },
            new LicenseTypeDefinition("GPL-2.0", "GNU GENERAL PUBLIC LICENSE", "Version 2")
            {
                // this is part of MPL, which lists some secondary licenses...
                ExclusionTextSnippets = new []{ "Secondary License" }
            },
            new LicenseTypeDefinition("GPL-3.0", "GNU GENERAL PUBLIC LICENSE", "Version 3")
            {
                // this is part of MPL, which lists some secondary licenses...
                ExclusionTextSnippets = new []{ "Secondary License" }
            },
            
            new LicenseTypeDefinition("Apache-1.0", "The Apache Group", "Redistribution and use in source and binary forms, with or without modification, are permitted provided that the following conditions are met"),
            new LicenseTypeDefinition("Apache-1.1", "Apache License", "1.1"),
            new LicenseTypeDefinition("Apache-2.0", "Apache License", "Version 2.0"),

            new LicenseTypeDefinition("BSD-2-Clause", "BSD 2-Clause License"),
            new LicenseTypeDefinition("BSD-2-Clause", "1.", "2.", "Redistribution and use in source and binary forms, with or without modification, are permitted provided that the following conditions are met")
            {
                ExclusionTextSnippets = new []{ "3.", "Neither the name of the copyright holder nor the names of its contributors may be used" }
            },
            new LicenseTypeDefinition("BSD-3-Clause", "BSD 3-Clause License"),
            new LicenseTypeDefinition("BSD-3-Clause", "1.", "2.", "3.", "Redistribution and use in source and binary forms, with or without modification, are permitted provided that the following conditions are met", "Neither the name of the copyright holder nor the names of its contributors may be used"),

            new LicenseTypeDefinition("ISC", "ISC License"),
            new LicenseTypeDefinition("Artistic-1.0", "The Artistic License"),
            
            new LicenseTypeDefinition("LGPL-2.0", "GNU LIBRARY GENERAL PUBLIC LICENSE ", "Version 2,")
            {
                // this is part of MPL, which lists some secondary licenses...
                ExclusionTextSnippets = new []{ "Secondary License" }
            },
            new LicenseTypeDefinition("LGPL-2.1", "GNU LESSER GENERAL PUBLIC LICENSE", "Version 2.1,")
            {
                // this is part of MPL, which lists some secondary licenses...
                ExclusionTextSnippets = new []{ "Secondary License" }
            },
            new LicenseTypeDefinition("LGPL-3.0", "GNU LESSER GENERAL PUBLIC LICENSE", "Version 3,")
            {
                // this is part of MPL, which lists some secondary licenses...
                ExclusionTextSnippets = new []{ "Secondary License" }
            },
            
            new LicenseTypeDefinition("EPL-1.0", "Eclipse Public License", "v 1.0"),
            new LicenseTypeDefinition("EPL-2.0", "Eclipse Public License", "v 2.0"),
            
            new LicenseTypeDefinition("MS-PL", "Microsoft Public License"),
            new LicenseTypeDefinition("MS-NETLIB", "MICROSOFT .NET LIBRARY"),
            
            new LicenseTypeDefinition("CPOL-1.02", "The Code Project Open License", "1.02"),
            
            new LicenseTypeDefinition("MPL-1.0", "MOZILLA PUBLIC LICENSE Version 1.0"),
            new LicenseTypeDefinition("MPL-1.0", "MOZILLA PUBLIC LICENSE 1.0"),
            new LicenseTypeDefinition("MPL-1.1", "Mozilla Public License Version 1.1"),
            new LicenseTypeDefinition("MPL-1.1", "Mozilla Public License 1.1"),
            new LicenseTypeDefinition("MPL-2.0", "Mozilla Public License Version 2.0"),
            new LicenseTypeDefinition("MPL-2.0", "Mozilla Public License 2.0"),
            
            new LicenseTypeDefinition("AGPL-1.0", "AFFERO GENERAL PUBLIC LICENSE", "Version 1")
            {
                // this is part of MPL, which lists some secondary licenses...
                ExclusionTextSnippets = new []{ "Secondary License" }
            },
            new LicenseTypeDefinition("AGPL-3.0", "GNU AFFERO GENERAL PUBLIC LICENSE", "Version 3")
            {
                // this is part of MPL, which lists some secondary licenses...
                ExclusionTextSnippets = new []{ "Secondary License" }
            },
            
            new LicenseTypeDefinition("CDDL-1.0", "COMMON DEVELOPMENT AND DISTRIBUTION LICENSE", "Version 1.0"),
            new LicenseTypeDefinition("CDDL-1.1", "COMMON DEVELOPMENT AND DISTRIBUTION LICENSE", "Version 1.1"),
            
            new LicenseTypeDefinition("WTFPL", "DO WHAT THE FUCK YOU WANT TO PUBLIC LICENSE "),
            new LicenseTypeDefinition("Zlib", "zlib License"),
            
            new LicenseTypeDefinition("Unlicense", "unlicense.org", "This is free and unencumbered software released into the public domain."),
            new LicenseTypeDefinition("ANTLR-PD", "ANTLR", "2"),
            new LicenseTypeDefinition("BSL-1.0", "Boost Software License", "Version 1.0"),
            new LicenseTypeDefinition("ICU", "ICU License"),
            new LicenseTypeDefinition("Info-ZIP", "Info-ZIP License"),
            new LicenseTypeDefinition("CPAL-1.0", "Common Public Attribution License", "Version 1.0"),
            new LicenseTypeDefinition("CPL-1.0", "Common Public License", "Version 1.0"),
            new LicenseTypeDefinition("IPL-1.0", "IBM Public License", "Version 1.0"),
            
            new LicenseTypeDefinition("NPL-1.0", "NETSCAPE PUBLIC LICENSE", "Version 1.0"),
            new LicenseTypeDefinition("NPL-1.1", "Netscape Public LIcense", "version 1.1"),
            
            new LicenseTypeDefinition("EUPL-1.0", "European Union Public Licence", "V.1.0"),
            new LicenseTypeDefinition("EUPL-1.1", "European Union Public Licence", "V.1.1"),
            new LicenseTypeDefinition("EUPL-1.2", "European Union Public Licence", "v. 1.2")
        };
    }
}
