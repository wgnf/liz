using Liz.Core.Logging;
using Liz.Core.Logging.Contracts;
using Liz.Core.PackageReferences.Contracts.Models;
using Liz.Core.Utils.Contracts;
using Liz.Core.Utils.Models;
using System.IO.Abstractions;

namespace Liz.Core.Utils;

internal sealed class FindPackageReferenceArtifact : IFindPackageReferenceArtifact
{
    private readonly IProvideNugetCacheDirectories _provideNugetCacheDirectories;
    private readonly IFileSystem _fileSystem;
    private readonly ILogger _logger;

    public FindPackageReferenceArtifact(
        IProvideNugetCacheDirectories provideNugetCacheDirectories,
        IFileSystem fileSystem,
        ILogger logger)
    {
        _provideNugetCacheDirectories = provideNugetCacheDirectories ??
                                        throw new ArgumentNullException(nameof(provideNugetCacheDirectories));
        _fileSystem = fileSystem ?? throw new ArgumentNullException(nameof(fileSystem));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<Optional<IDirectoryInfo>> TryGetArtifactAsync(PackageReference packageReference)
    {
        if (packageReference == null) throw new ArgumentNullException(nameof(packageReference));

        var nugetCaches = await _provideNugetCacheDirectories.GetAsync().ConfigureAwait(false);
        _logger.LogDebug($"Searching artifact for {packageReference}...");

        foreach (var nugetCache in nugetCaches)
        {
            if (!TryGetArtifactDirectoryFromCache(nugetCache, packageReference, out var artifactDirectoryCandidate) || 
                artifactDirectoryCandidate == null) 
                continue;

            _logger.LogDebug($"Found artifact at {artifactDirectoryCandidate.FullName}");
            return Optional<IDirectoryInfo>.Success(artifactDirectoryCandidate);
        }
        
        _logger.LogDebug("Could not find artifact");
        return Optional<IDirectoryInfo>.Failure();
    }

    private bool TryGetArtifactDirectoryFromCache(
        string? nugetCache, 
        PackageReference packageReference,
        out IDirectoryInfo? artifactDirectory)
    {
        var artifactCandidate = _fileSystem
            .Path
            .Combine(nugetCache, packageReference.Name.ToLower(), packageReference.Version.ToLower());
        
        _logger.LogDebug($"Candidate: {artifactCandidate}");
        
        artifactDirectory = _fileSystem.DirectoryInfo.FromDirectoryName(artifactCandidate);

        return artifactDirectory is { Exists: true };
    }
}
