using Liz.Core.License.Contracts;
using Liz.Core.License.Contracts.Models;
using Liz.Core.Logging;
using Liz.Core.Logging.Contracts;
using Liz.Core.PackageReferences.Contracts;
using Liz.Core.PackageReferences.Contracts.Models;
using System.IO.Abstractions;

namespace Liz.Core.License;

internal sealed class EnrichPackageReferenceWithLicenseInformation : IEnrichPackageReferenceWithLicenseInformation
{
    private readonly IGetDownloadedPackageReferenceArtifact _getDownloadedPackageReferenceArtifact;
    private readonly IGetLicenseInformationFromArtifact _getLicenseInformationFromArtifact;
    private readonly ILogger _logger;

    public EnrichPackageReferenceWithLicenseInformation(
        IGetLicenseInformationFromArtifact getLicenseInformationFromArtifact,
        ILogger logger,
        IGetDownloadedPackageReferenceArtifact getDownloadedPackageReferenceArtifact)
    {
        _getLicenseInformationFromArtifact = getLicenseInformationFromArtifact ??
                                             throw new ArgumentNullException(nameof(getLicenseInformationFromArtifact));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _getDownloadedPackageReferenceArtifact = getDownloadedPackageReferenceArtifact ??
                                                 throw new ArgumentNullException(
                                                     nameof(getDownloadedPackageReferenceArtifact));
    }

    public async Task EnrichAsync(PackageReference packageReference)
    {
        ArgumentNullException.ThrowIfNull(packageReference);

        if (!_getDownloadedPackageReferenceArtifact.TryGetFor(
                packageReference,
                out var downloadedPackageReferenceDirectory) || downloadedPackageReferenceDirectory == null)
        {
            var message = $"Could not find downloaded artifacts for {packageReference}, this can be due to:{Environment.NewLine}" +
                          $"- it actually being a project-reference{Environment.NewLine}" +
                          "- it being a manually added reference which was not being downloaded via restoring";
            
            _logger.LogDebug(message);
            return;
        }

        var licenseInformation = await GetLicenseInformationAsync(downloadedPackageReferenceDirectory);
        packageReference.LicenseInformation = licenseInformation;
    }

    private async Task<LicenseInformation> GetLicenseInformationAsync(
        IDirectoryInfo downloadedPackageReferenceDirectory)
    {
        _logger.LogDebug($"Determining license information from {downloadedPackageReferenceDirectory}...");
        var licenseInformation =
            await _getLicenseInformationFromArtifact.GetFromDownloadedPackageReferenceAsync(
                downloadedPackageReferenceDirectory);

        return licenseInformation;
    }
}
