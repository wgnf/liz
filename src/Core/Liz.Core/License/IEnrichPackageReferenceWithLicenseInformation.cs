using Liz.Core.PackageReferences;
using System.Threading.Tasks;

namespace Liz.Core.License;

internal interface IEnrichPackageReferenceWithLicenseInformation
{
    Task EnrichAsync(PackageReference packageReference);
}
