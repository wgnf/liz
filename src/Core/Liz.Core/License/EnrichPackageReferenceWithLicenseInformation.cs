using Liz.Core.License.Contracts;
using Liz.Core.License.Contracts.Models;
using Liz.Core.Logging;
using Liz.Core.Logging.Contracts;
using Liz.Core.PackageReferences.Contracts.Models;
using System.IO.Abstractions;

namespace Liz.Core.License;

internal sealed class EnrichPackageReferenceWithLicenseInformation : IEnrichPackageReferenceWithLicenseInformation
{
    private readonly IGetLicenseInformationFromArtifact _getLicenseInformationFromArtifact;
    private readonly ILogger _logger;
    private readonly IFileSystem _fileSystem;

    public EnrichPackageReferenceWithLicenseInformation(
        IGetLicenseInformationFromArtifact getLicenseInformationFromArtifact,
        ILogger logger,
        IFileSystem fileSystem)
    {
        _getLicenseInformationFromArtifact = getLicenseInformationFromArtifact ??
                                             throw new ArgumentNullException(nameof(getLicenseInformationFromArtifact));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _fileSystem = fileSystem ?? throw new ArgumentNullException(nameof(fileSystem));
    }

    public async Task EnrichAsync(PackageReference packageReference)
    {
        if (packageReference == null) throw new ArgumentNullException(nameof(packageReference));
        if (packageReference.ArtifactDirectory == null) return;

        var artifactDirectory = _fileSystem.DirectoryInfo.FromDirectoryName(packageReference.ArtifactDirectory);

        var licenseInformation = await GetLicenseInformationAsync(artifactDirectory).ConfigureAwait(false);
        packageReference.LicenseInformation = licenseInformation;
    }

    private async Task<LicenseInformation> GetLicenseInformationAsync(IDirectoryInfo artifactDirectory)
    {
        _logger.LogDebug($"Determining license information from {artifactDirectory}...");
        var licenseInformation =
            await _getLicenseInformationFromArtifact.GetFromDownloadedPackageReferenceAsync(
                artifactDirectory).ConfigureAwait(false);

        return licenseInformation;
    }
}
