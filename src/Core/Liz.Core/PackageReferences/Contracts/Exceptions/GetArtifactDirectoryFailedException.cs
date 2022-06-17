using Liz.Core.PackageReferences.Contracts.Models;
using System.Diagnostics.CodeAnalysis;

namespace Liz.Core.PackageReferences.Contracts.Exceptions;

/// <summary>
///     <see cref="Exception"/> that occurs when the tool failed to determine an artifact directory for a given target
///     <see cref="PackageReference"/>
/// </summary>
[ExcludeFromCodeCoverage] // simple exception
public class GetArtifactDirectoryFailedException : Exception
{
    internal GetArtifactDirectoryFailedException(PackageReference packageReference, Exception? innerException = null)
        :base($"Unable to determine artifact directory for package-reference {packageReference}", innerException)
    {
    }
}
