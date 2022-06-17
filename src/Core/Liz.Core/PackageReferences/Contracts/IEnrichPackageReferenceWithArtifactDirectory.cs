using Liz.Core.PackageReferences.Contracts.Models;

namespace Liz.Core.PackageReferences.Contracts;

internal interface IEnrichPackageReferenceWithArtifactDirectory
{
    Task EnrichAsync(PackageReference packageReference);
}
