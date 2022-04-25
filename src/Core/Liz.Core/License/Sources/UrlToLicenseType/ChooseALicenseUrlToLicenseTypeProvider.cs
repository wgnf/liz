using Liz.Core.License.Contracts;

// ReSharper disable StringLiteralTypo

namespace Liz.Core.License.Sources.UrlToLicenseType;

// c.f.: https://choosealicense.com/licenses/
internal sealed class ChooseALicenseUrlToLicenseTypeProvider : IUrlToLicenseTypeMappingProvider
{
    public IDictionary<string, string> Get()
    {
        return new Dictionary<string, string>
        {
            { "https://choosealicense.com/licenses/agpl-3.0/", "AGPL-3.0" },
            { "https://choosealicense.com/licenses/gpl-3.0/", "GPL-3.0" },
            { "https://choosealicense.com/licenses/lgpl-3.0/", "LGPL-3.0" },
            { "https://choosealicense.com/licenses/mpl-2.0/", "MPL-2.0" },
            { "https://choosealicense.com/licenses/apache-2.0/", "Apache-2.0" },
            { "https://choosealicense.com/licenses/mit/", "MIT" },
            { "https://choosealicense.com/licenses/bsl-1.0/", "BSL-1.0" },
            { "https://choosealicense.com/licenses/unlicense/", "Unlicense" },
            
            // an exact copy of the links above, just using http instead of https (as it seems some are still using http)
            { "http://choosealicense.com/licenses/agpl-3.0/", "AGPL-3.0" },
            { "http://choosealicense.com/licenses/gpl-3.0/", "GPL-3.0" },
            { "http://choosealicense.com/licenses/lgpl-3.0/", "LGPL-3.0" },
            { "http://choosealicense.com/licenses/mpl-2.0/", "MPL-2.0" },
            { "http://choosealicense.com/licenses/apache-2.0/", "Apache-2.0" },
            { "http://choosealicense.com/licenses/mit/", "MIT" },
            { "http://choosealicense.com/licenses/bsl-1.0/", "BSL-1.0" },
            { "http://choosealicense.com/licenses/unlicense/", "Unlicense" }
        };
    }
}
