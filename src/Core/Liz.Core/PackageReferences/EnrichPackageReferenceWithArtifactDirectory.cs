using Liz.Core.PackageReferences.Contracts;
using Liz.Core.PackageReferences.Contracts.Models;
using Liz.Core.Utils.Contracts;

namespace Liz.Core.PackageReferences;

internal sealed class EnrichPackageReferenceWithArtifactDirectory : IEnrichPackageReferenceWithArtifactDirectory
{
    private readonly IFindPackageReferenceArtifact _findPackageReferenceArtifact;

    public EnrichPackageReferenceWithArtifactDirectory(
        IFindPackageReferenceArtifact findPackageReferenceArtifact)
    {
        _findPackageReferenceArtifact = findPackageReferenceArtifact 
                                        ?? throw new ArgumentNullException(nameof(findPackageReferenceArtifact));
    }
    
    public async Task EnrichAsync(PackageReference packageReference)
    {
        if (packageReference == null) throw new ArgumentNullException(nameof(packageReference));

        var artifactDirectory =
            await _findPackageReferenceArtifact.TryGetArtifactAsync(packageReference).ConfigureAwait(false);
        if (!artifactDirectory.HasResult) return;

        packageReference.ArtifactDirectory = artifactDirectory.Result?.FullName;
    }
}
