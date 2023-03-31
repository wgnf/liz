using System.Diagnostics.CodeAnalysis;
using Liz.Core.PackageReferences.Contracts.Models;

namespace Liz.Core.PackageReferences.Contracts.Exceptions;

/// <summary>
///     <see cref="Exception" /> that occurs when the tool failed to determine a <see cref="PackageReference" />
/// </summary>
[ExcludeFromCodeCoverage] // simple exception
public class GetPackageReferenceFailedException : Exception
{
    /// <summary>
    ///     Create a new instance of <see cref="GetPackageReferencesFailedException" />
    /// </summary>
    /// <param name="line">The line which couldn't be parsed to a <see cref="PackageReference" /></param>
    /// <param name="reason">
    ///     The reason why hte given <paramref name="line" /> couldn't be parsed to a
    ///     <see cref="PackageReference" />
    /// </param>
    /// <param name="innerException">An optional inner <see cref="Exception" /></param>
    internal GetPackageReferenceFailedException(string line, string reason, Exception? innerException = null)
        : base($"Unable to parse package-reference from line '{line}', because: {reason}", innerException)
    {
    }
}
