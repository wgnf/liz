using JetBrains.Annotations;
using Liz.Core.PackageReferences.Contracts.Models;
using System.IO.Abstractions;

namespace Liz.Core.PackageReferences.Contracts;

internal interface IGetDownloadedPackageReferenceArtifact
{
    bool TryGetFor(
        [NotNull] PackageReference packageReference, 
        [CanBeNull] out IDirectoryInfo packageReferenceDownloadDirectory);
}
