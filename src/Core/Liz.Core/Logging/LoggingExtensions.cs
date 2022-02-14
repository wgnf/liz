using Liz.Core.Logging.Contracts;

namespace Liz.Core.Logging;

/// <summary>
///     Extensions for the liz <see cref="ILogger"/> interface
/// </summary>
public static class LoggingExtensions
{
    /// <summary>
    ///     Issues a log on the <see cref="LogLevel.Trace"/> level
    /// </summary>
    /// <param name="logger">The logger instance</param>
    /// <param name="message">The message to log</param>
    /// <param name="exception">An additional <see cref="Exception"/> to log</param>
    public static void LogTrace(this ILogger logger, string message, Exception? exception = null)
    {
        ArgumentNullException.ThrowIfNull(logger);
        
        logger.Log(LogLevel.Trace, message, exception);
    }

    /// <summary>
    ///     Issues a log on the <see cref="LogLevel.Debug"/> level
    /// </summary>
    /// <param name="logger">The logger instance</param>
    /// <param name="message">The message to log</param>
    /// <param name="exception">An additional <see cref="Exception"/> to log</param>
    public static void LogDebug(this ILogger logger, string message, Exception? exception = null)
    {
        ArgumentNullException.ThrowIfNull(logger);

        logger.Log(LogLevel.Debug, message, exception);
    }

    /// <summary>
    ///     Issues a log on the <see cref="LogLevel.Information"/> level
    /// </summary>
    /// <param name="logger">The logger instance</param>
    /// <param name="message">The message to log</param>
    /// <param name="exception">An additional <see cref="Exception"/> to log</param>
    public static void LogInformation(this ILogger logger, string message, Exception? exception = null)
    {
        ArgumentNullException.ThrowIfNull(logger);
        
        logger.Log(LogLevel.Information, message, exception);
    }

    /// <summary>
    ///     Issues a log on the <see cref="LogLevel.Warning"/> level
    /// </summary>
    /// <param name="logger">The logger instance</param>
    /// <param name="message">The message to log</param>
    /// <param name="exception">An additional <see cref="Exception"/> to log</param>
    public static void LogWarning(this ILogger logger, string message, Exception? exception = null)
    {
        ArgumentNullException.ThrowIfNull(logger);
        
        logger.Log(LogLevel.Warning, message, exception);
    }

    /// <summary>
    ///     Issues a log on the <see cref="LogLevel.Error"/> level
    /// </summary>
    /// <param name="logger">The logger instance</param>
    /// <param name="message">The message to log</param>
    /// <param name="exception">An additional <see cref="Exception"/> to log</param>
    public static void LogError(this ILogger logger, string message, Exception? exception = null)
    {
        ArgumentNullException.ThrowIfNull(logger);
        
        logger.Log(LogLevel.Error, message, exception);
    }

    /// <summary>
    ///     Issues a log on the <see cref="LogLevel.Critical"/> level
    /// </summary>
    /// <param name="logger">The logger instance</param>
    /// <param name="message">The message to log</param>
    /// <param name="exception">An additional <see cref="Exception"/> to log</param>
    public static void LogCritical(this ILogger logger, string message, Exception? exception = null)
    {
        ArgumentNullException.ThrowIfNull(logger);
        
        logger.Log(LogLevel.Critical, message, exception);
    }
}
