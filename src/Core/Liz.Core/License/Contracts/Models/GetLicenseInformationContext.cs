using System.Diagnostics.CodeAnalysis;
using System.IO.Abstractions;
using System.Xml.Linq;

namespace Liz.Core.License.Contracts.Models;

[ExcludeFromCodeCoverage] // DTO
internal sealed class GetLicenseInformationContext
{
    public GetLicenseInformationContext()
    {
        LicenseInformation = new LicenseInformation();
    }
    
    public IDirectoryInfo ArtifactDirectory { get; init; }

    public XDocument NugetSpecificationFileXml { get; init; }

    public LicenseInformation LicenseInformation { get; }
}
