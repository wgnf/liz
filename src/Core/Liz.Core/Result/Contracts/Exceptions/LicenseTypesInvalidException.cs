using System.Diagnostics.CodeAnalysis;

namespace Liz.Core.Result.Contracts.Exceptions;

/// <summary>
///     <see cref="Exception"/> that occurs when validating the license-type of packages and the validation determined
///     that the license-types are invalid
/// </summary>
[ExcludeFromCodeCoverage] // simple exception
public sealed class LicenseTypesInvalidException : Exception
{
    /// <summary>
    ///     Creates a new instance of <see cref="LicenseTypesInvalidException"/>
    /// </summary>
    /// <param name="message">The message that describes why the license-types are invalid</param>
    public LicenseTypesInvalidException(string message) : base(message)
    {
    }
}
