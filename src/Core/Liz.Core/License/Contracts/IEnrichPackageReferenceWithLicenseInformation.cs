using Liz.Core.PackageReferences.Contracts.Models;

namespace Liz.Core.License.Contracts;

internal interface IEnrichPackageReferenceWithLicenseInformation
{
    Task EnrichAsync(PackageReference packageReference);
}
