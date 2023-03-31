using System.IO.Abstractions;
using Liz.Core.License.Contracts.Models;

namespace Liz.Core.License.Contracts;

internal interface IGetLicenseInformationFromArtifact
{
    Task<LicenseInformation> GetFromDownloadedPackageReferenceAsync(IDirectoryInfo downloadDirectory);
}
