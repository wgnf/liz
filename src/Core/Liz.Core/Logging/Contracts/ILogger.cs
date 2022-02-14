namespace Liz.Core.Logging.Contracts;

/// <summary>
///     The logger interface for liz
/// </summary>
public interface ILogger
{
    /// <summary>
    ///     Log a message to whatever source with the given values
    /// </summary>
    /// <param name="level">The <see cref="LogLevel"/> you want to log at</param>
    /// <param name="message">The message that you want to log</param>
    /// <param name="exception">The <see cref="Exception"/> that you want to additionally log</param>
    public void Log(LogLevel level, string message, Exception? exception = null);
}
