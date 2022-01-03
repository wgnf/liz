using System.Diagnostics.CodeAnalysis;
using System.IO.Abstractions;
using System.Xml.Linq;

namespace Liz.Core.License;

[ExcludeFromCodeCoverage] // simple DTO
internal sealed class GetLicenseInformationResult
{
    public IDirectoryInfo ArtifactDirectory { get; set; }

    public XDocument NugetSpecifiactionFileXml { get; set; }
    
    public string LicenseType { get; set; }

    public string LicenseUrl { get; set; }

    public string RawLicenseText { get; set; }
}
