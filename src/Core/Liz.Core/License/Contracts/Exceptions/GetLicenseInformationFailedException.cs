using Liz.Core.License.Contracts.Models;
using Liz.Core.PackageReferences.Contracts.Models;
using System;
using System.Diagnostics.CodeAnalysis;

namespace Liz.Core.License.Contracts.Exceptions;

/// <summary>
///     <see cref="Exception" /> that occurs when the tool failed to determine <see cref="LicenseInformation" /> for a
///     given target <see cref="PackageReference" />
/// </summary>
[ExcludeFromCodeCoverage] // simple exception
public sealed class GetLicenseInformationFailedException : Exception
{
    /// <summary>
    ///     Create a new instance of <see cref="GetLicenseInformationFailedException" />
    /// </summary>
    /// <param name="packageReference">
    ///     The <see cref="PackageReference" /> which was used to determine the
    ///     <see cref="LicenseInformation" />
    /// </param>
    /// <param name="innerException">An optional inner <see cref="Exception" /></param>
    internal GetLicenseInformationFailedException(PackageReference packageReference, Exception? innerException = null)
        : base($"Unable to determine license information for package-reference {packageReference}", innerException)
    {
    }
}
