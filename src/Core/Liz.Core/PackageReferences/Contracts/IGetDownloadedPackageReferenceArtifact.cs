using Liz.Core.PackageReferences.Contracts.Models;
using System.Diagnostics.CodeAnalysis;
using System.IO.Abstractions;

namespace Liz.Core.PackageReferences.Contracts;

internal interface IGetDownloadedPackageReferenceArtifact
{
    bool TryGetFor([NotNull] PackageReference packageReference, out IDirectoryInfo packageReferenceDownloadDirectory);
}
