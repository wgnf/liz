using JetBrains.Annotations;
using Liz.Core.License.Contracts;
using Liz.Core.License.Contracts.Models;
using Liz.Core.Logging;
using Liz.Core.Logging.Contracts;
using Liz.Core.PackageReferences.Contracts;
using Liz.Core.PackageReferences.Contracts.Models;
using System;
using System.IO.Abstractions;
using System.Threading.Tasks;

namespace Liz.Core.License;

internal sealed class EnrichPackageReferenceWithLicenseInformation : IEnrichPackageReferenceWithLicenseInformation
{
    private readonly IDownloadPackageReference _downloadPackageReference;
    private readonly IGetLicenseInformationFromArtifact _getLicenseInformationFromArtifact;
    private readonly ILogger _logger;

    public EnrichPackageReferenceWithLicenseInformation(
        [NotNull] IDownloadPackageReference downloadPackageReference,
        [NotNull] IGetLicenseInformationFromArtifact getLicenseInformationFromArtifact,
        [NotNull] ILogger logger)
    {
        _downloadPackageReference = downloadPackageReference ?? throw new ArgumentNullException(nameof(downloadPackageReference));
        _getLicenseInformationFromArtifact = getLicenseInformationFromArtifact ?? throw new ArgumentNullException(nameof(getLicenseInformationFromArtifact));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }
    
    public async Task EnrichAsync(PackageReference packageReference)
    {
        ArgumentNullException.ThrowIfNull(packageReference);

        var downloadedPackageReferenceDirectory = await DownloadPackageReferenceAsync(packageReference);
        var licenseInformation = await GetLicenseInformationAsync(downloadedPackageReferenceDirectory);

        packageReference.LicenseInformation = licenseInformation;
    }
    
    private async Task<IDirectoryInfo> DownloadPackageReferenceAsync(PackageReference packageReference)
    {
        _logger.LogDebug($"Downloading {packageReference}...");
        var downloadedPackageReferenceDirectory = await _downloadPackageReference.DownloadAsync(packageReference);
        _logger.LogDebug($"Downloaded {packageReference} to {downloadedPackageReferenceDirectory}");
        
        return downloadedPackageReferenceDirectory;
    }
    
    private async Task<LicenseInformation> GetLicenseInformationAsync(IDirectoryInfo downloadedPackageReferenceDirectory)
    {
        _logger.LogDebug($"Determining license information from {downloadedPackageReferenceDirectory}...");
        var licenseInformation =
            await _getLicenseInformationFromArtifact.GetFromDownloadedPackageReferenceAsync(
                downloadedPackageReferenceDirectory);

        return licenseInformation;
    }
}
