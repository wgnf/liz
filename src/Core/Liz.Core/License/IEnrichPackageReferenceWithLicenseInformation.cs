using Liz.Core.PackageReferences;
using System.Threading.Tasks;

namespace Liz.Core.License;

internal interface IEnrichPackageReferenceWithLicenseInformation
{
    Task GetFromPackageReferenceAsync(PackageReference packageReference);
}
