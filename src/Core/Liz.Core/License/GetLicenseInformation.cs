using JetBrains.Annotations;
using Liz.Core.Logging;
using Liz.Core.PackageReferences;
using System;
using System.IO.Abstractions;
using System.Threading.Tasks;

namespace Liz.Core.License;

internal sealed class GetLicenseInformation : IGetLicenseInformation
{
    private readonly IDownloadPackageReference _downloadPackageReference;
    private readonly IGetLicenseInformationFromArtifact _getLicenseInformationFromArtifact;
    private readonly ILogger _logger;

    public GetLicenseInformation(
        [NotNull] IDownloadPackageReference downloadPackageReference,
        [NotNull] IGetLicenseInformationFromArtifact getLicenseInformationFromArtifact,
        [NotNull] ILogger logger)
    {
        _downloadPackageReference = downloadPackageReference ?? throw new ArgumentNullException(nameof(downloadPackageReference));
        _getLicenseInformationFromArtifact = getLicenseInformationFromArtifact ?? throw new ArgumentNullException(nameof(getLicenseInformationFromArtifact));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }
    
    public async Task<LicenseInformation> GetFromPackageReferenceAsync(PackageReference packageReference)
    {
        ArgumentNullException.ThrowIfNull(packageReference);

        var downloadedPackageReferenceDirectory = await DownloadPackageReferenceAsync(packageReference);
        var licenseInformationResult = await GetLicenseInformationAsync(downloadedPackageReferenceDirectory);

        var licenseInformation = new LicenseInformation(
            licenseInformationResult.LicenseType,
            licenseInformationResult.LicenseUrl, 
            licenseInformationResult.RawLicenseText, 
            packageReference);
        return licenseInformation;
    }
    
    private async Task<IDirectoryInfo> DownloadPackageReferenceAsync(PackageReference packageReference)
    {
        _logger.LogDebug($"Downloading {packageReference}...");
        var downloadedPackageReferenceDirectory = await _downloadPackageReference.DownloadAsync(packageReference);
        _logger.LogDebug($"Downloaded {packageReference} to {downloadedPackageReferenceDirectory}");
        
        return downloadedPackageReferenceDirectory;
    }
    
    private async Task<GetLicenseInformationResult> GetLicenseInformationAsync(IDirectoryInfo downloadedPackageReferenceDirectory)
    {
        _logger.LogDebug($"Determining license information from {downloadedPackageReferenceDirectory}...");
        var licenseInformationResult =
            await _getLicenseInformationFromArtifact.GetFromDownloadedPackageReferenceAsync(
                downloadedPackageReferenceDirectory);

        return licenseInformationResult;
    }
}
