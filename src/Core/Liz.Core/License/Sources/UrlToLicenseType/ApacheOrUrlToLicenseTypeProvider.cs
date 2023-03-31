using Liz.Core.License.Contracts;

namespace Liz.Core.License.Sources.UrlToLicenseType;

internal sealed class ApacheOrUrlToLicenseTypeProvider : IUrlToLicenseTypeMappingProvider
{
    public IDictionary<string, string> Get()
    {
        return new Dictionary<string, string>
        {
            { "http://www.apache.org/licenses/LICENSE-2.0.html", "Apache-2.0" },
            { "http://www.apache.org/licenses/LICENSE-2.0", "Apache-2.0" },
            { "https://www.apache.org/licenses/LICENSE-2.0.html", "Apache-2.0" },
            { "https://www.apache.org/licenses/LICENSE-2.0", "Apache-2.0" }
        };
    }
}
