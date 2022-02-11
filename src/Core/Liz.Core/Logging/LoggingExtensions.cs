using Liz.Core.Logging.Contracts;

namespace Liz.Core.Logging;

public static class LoggingExtensions
{
    public static void LogTrace(this ILogger logger, string message, Exception? exception = null)
    {
        ArgumentNullException.ThrowIfNull(logger);
        
        logger.Log(LogLevel.Trace, message, exception);
    }

    public static void LogDebug(this ILogger logger, string message, Exception? exception = null)
    {
        ArgumentNullException.ThrowIfNull(logger);

        logger.Log(LogLevel.Debug, message, exception);
    }

    public static void LogInformation(this ILogger logger, string message, Exception? exception = null)
    {
        ArgumentNullException.ThrowIfNull(logger);
        
        logger.Log(LogLevel.Information, message, exception);
    }

    public static void LogWarning(this ILogger logger, string message, Exception? exception = null)
    {
        ArgumentNullException.ThrowIfNull(logger);
        
        logger.Log(LogLevel.Warning, message, exception);
    }

    public static void LogError(this ILogger logger, string message, Exception? exception = null)
    {
        ArgumentNullException.ThrowIfNull(logger);
        
        logger.Log(LogLevel.Error, message, exception);
    }

    public static void LogCritical(this ILogger logger, string message, Exception? exception = null)
    {
        ArgumentNullException.ThrowIfNull(logger);
        
        logger.Log(LogLevel.Critical, message, exception);
    }
}
