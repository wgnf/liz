using Liz.Core.License.Contracts;

// ReSharper disable StringLiteralTypo

namespace Liz.Core.License.Sources.LicenseType;

internal sealed class ApacheLicenseTypeDefinitionProvider : ILicenseTypeDefinitionProvider
{
    public IEnumerable<LicenseTypeDefinition> Get()
    {
        return new[]
        {
            new LicenseTypeDefinition("Apache-1.0", "The Apache Group", "Redistribution and use in source and binary forms, with or without modification, are permitted provided that the following conditions are met"),
            new LicenseTypeDefinition("Apache-1.1", "Apache License", "1.1"),
            new LicenseTypeDefinition("Apache-2.0", "Apache License", "Version 2.0")
        };
    }
}
