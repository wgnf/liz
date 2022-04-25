using System.Diagnostics.CodeAnalysis;

namespace Liz.Core.Preparation.Contracts.Exceptions;

/// <summary>
///     <see cref="Exception"/> that occurs when the preparation to run Liz failed due to an error
/// </summary>
[ExcludeFromCodeCoverage] // simple exception
public sealed class PreparationFailedException : Exception
{
    /// <summary>
    ///     Create a new instance of <see cref="PreparationFailedException"/>
    /// </summary>
    /// <param name="innerException">An optional inner <see cref="Exception"/></param>
    public PreparationFailedException(Exception? innerException = null)
        :base("Preparation to run Liz failed", innerException)
    {
    }
}
