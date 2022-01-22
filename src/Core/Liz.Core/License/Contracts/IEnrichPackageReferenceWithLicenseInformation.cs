using Liz.Core.PackageReferences.Contracts.Models;
using System.Threading.Tasks;

namespace Liz.Core.License.Contracts;

internal interface IEnrichPackageReferenceWithLicenseInformation
{
    Task EnrichAsync(PackageReference packageReference);
}
