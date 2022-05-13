using System.Diagnostics.CodeAnalysis;

namespace Liz.Core.Settings;

/// <summary>
///     An <see cref="Exception"/> that is thrown when some settings are not valid
/// </summary>
[ExcludeFromCodeCoverage] // simple Exception
public class SettingsInvalidException : Exception
{
    /// <summary>
    ///     Creates a new instance of <see cref="SettingsInvalidException"/> with a given message
    /// </summary>
    /// <param name="message">Message that describes the issue</param>
    /// <param name="innerException">An optional inner <see cref="Exception"/></param>
    internal SettingsInvalidException(string message, Exception? innerException = null) : base(message, innerException)
    {
    }
}
