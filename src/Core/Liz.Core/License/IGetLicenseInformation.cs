using Liz.Core.PackageReferences;
using System.Threading.Tasks;

namespace Liz.Core.License;

internal interface IGetLicenseInformation
{
    Task<LicenseInformation> GetFromPackageReferenceAsync(PackageReference packageReference);
}
