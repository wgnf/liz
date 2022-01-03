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
    private readonly ILogger _logger;

    public GetLicenseInformation(
        [NotNull] IDownloadPackageReference downloadPackageReference,
        [NotNull] ILogger logger)
    {
        _downloadPackageReference = downloadPackageReference ?? throw new ArgumentNullException(nameof(downloadPackageReference));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }
    
    public async Task<LicenseInformation> GetFromPackageReferenceAsync(PackageReference packageReference)
    {
        ArgumentNullException.ThrowIfNull(packageReference);

        var downloadedPackageReferenceDirectory = await DownloadPackageReferenceAsync(packageReference);
    }

    // TODO: HOW TO HANDLE Project-References?!
    private async Task<IDirectoryInfo> DownloadPackageReferenceAsync(PackageReference packageReference)
    {
        _logger.LogDebug($"Downloading {packageReference}...");
        var downloadedPackageReferenceDirectory = await _downloadPackageReference.DownloadAsync(packageReference);
        _logger.LogDebug($"Downloaded {packageReference} to {downloadedPackageReferenceDirectory}");
        
        return downloadedPackageReferenceDirectory;
    }
    

        throw new NotImplementedException();
    }
}
