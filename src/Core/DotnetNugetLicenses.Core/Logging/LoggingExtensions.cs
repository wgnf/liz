using JetBrains.Annotations;
using System;

namespace DotnetNugetLicenses.Core.Logging;

[PublicAPI]
public static class LoggingExtensions
{
    public static void LogTrace([NotNull] this ILogger logger, string message, Exception exception = null)
    {
        ArgumentNullException.ThrowIfNull(logger);
        
        logger.Log(LogLevel.Trace, message, exception);
    }

    public static void LogDebug([NotNull] this ILogger logger, string message, Exception exception = null)
    {
        ArgumentNullException.ThrowIfNull(logger);

        logger.Log(LogLevel.Debug, message, exception);
    }

    public static void LogInformation([NotNull] this ILogger logger, string message, Exception exception = null)
    {
        ArgumentNullException.ThrowIfNull(logger);
        
        logger.Log(LogLevel.Information, message, exception);
    }

    public static void LogWarning([NotNull] this ILogger logger, string message, Exception exception = null)
    {
        ArgumentNullException.ThrowIfNull(logger);
        
        logger.Log(LogLevel.Warning, message, exception);
    }

    public static void LogError([NotNull] this ILogger logger, string message, Exception exception = null)
    {
        ArgumentNullException.ThrowIfNull(logger);
        
        logger.Log(LogLevel.Error, message, exception);
    }

    public static void LogCritical([NotNull] this ILogger logger, string message, Exception exception = null)
    {
        ArgumentNullException.ThrowIfNull(logger);
        
        logger.Log(LogLevel.Critical, message, exception);
    }
}
