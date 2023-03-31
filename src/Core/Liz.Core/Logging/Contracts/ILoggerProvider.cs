namespace Liz.Core.Logging.Contracts;

/// <summary>
///     The logger provider interface for liz
/// </summary>
public interface ILoggerProvider
{
    /// <summary>
    ///     Provides the desired <see cref="ILogger" /> instance
    /// </summary>
    /// <param name="logLevel">The activated <see cref="LogLevel" /></param>
    /// <returns>The provided <see cref="ILogger" /> instance</returns>
    ILogger Get(LogLevel logLevel);
}
