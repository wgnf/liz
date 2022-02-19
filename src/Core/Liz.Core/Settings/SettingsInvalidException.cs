namespace Liz.Core.Settings;

/// <summary>
///     An <see cref="Exception"/> that is thrown when some settings are not valid
/// </summary>
public class SettingsInvalidException : Exception
{
    /// <summary>
    ///     Creates a new instance of <see cref="SettingsInvalidException"/> with a given message
    /// </summary>
    /// <param name="message">Message that describes the issue</param>
    internal SettingsInvalidException(string message) : base(message)
    {
    }
}
