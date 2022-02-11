using Liz.Core.License.Contracts.Models;
using System.IO.Abstractions;

namespace Liz.Core.License.Contracts;

internal interface IGetLicenseInformationFromArtifact
{
    Task<LicenseInformation> GetFromDownloadedPackageReferenceAsync(IDirectoryInfo downloadDirectory);
}
