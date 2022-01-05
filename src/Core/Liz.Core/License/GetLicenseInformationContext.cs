using System.IO.Abstractions;
using System.Xml.Linq;

namespace Liz.Core.License;

internal sealed class GetLicenseInformationContext
{
    public GetLicenseInformationContext()
    {
        LicenseInformation = new LicenseInformation();
    }
    
    public IDirectoryInfo ArtifactDirectory { get; set; }

    public XDocument NugetSpecificationFileXml { get; set; }

    public LicenseInformation LicenseInformation { get; }
}
