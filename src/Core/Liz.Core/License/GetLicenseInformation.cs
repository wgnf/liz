using Liz.Core.PackageReferences;
using System;
using System.Threading.Tasks;

namespace Liz.Core.License;

internal sealed class GetLicenseInformation : IGetLicenseInformation
{
    public Task<LicenseInformation> GetFromPackageReferenceAsync(PackageReference packageReference)
    {
        ArgumentNullException.ThrowIfNull(packageReference);
        
        // download...
        // determine license type and raw license text

        throw new NotImplementedException();
    }
}
