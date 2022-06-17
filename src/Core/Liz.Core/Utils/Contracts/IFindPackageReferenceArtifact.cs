using Liz.Core.PackageReferences.Contracts.Models;
using Liz.Core.Utils.Models;
using System.IO.Abstractions;

namespace Liz.Core.Utils.Contracts;

internal interface IFindPackageReferenceArtifact
{
    Task<Optional<IDirectoryInfo>> TryGetArtifactAsync(PackageReference packageReference);
}
