using System;
using System.Diagnostics.CodeAnalysis;

namespace Liz.Core.PackageReferences;

/// <summary>
///     <see cref="Exception" /> that occurs when the tool attempts to download a given <see cref="PackageReference" />
/// </summary>
[ExcludeFromCodeCoverage] // simple exception
public class DownloadPackageReferenceFailedException : Exception
{
    /// <summary>
    ///     Create a new instance of <see cref="DownloadPackageReferenceFailedException" />
    /// </summary>
    /// <param name="packageReference">The <see cref="PackageReference" /> that was attempted to be downloaded</param>
    /// <param name="innerException">An optional inner <see cref="Exception" /></param>
    internal DownloadPackageReferenceFailedException(PackageReference packageReference, Exception innerException = null)
        : base($"Unable to download {packageReference}", innerException)
    {
    }
}
