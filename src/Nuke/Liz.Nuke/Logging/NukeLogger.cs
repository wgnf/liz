using Liz.Core.Logging.Contracts;
using Serilog.Events;
using System.Diagnostics.CodeAnalysis;
using ILogger = Liz.Core.Logging.Contracts.ILogger;

namespace Liz.Nuke.Logging;

/*
 * not really testable because of static dependency on Serilog.Log, but that's unfortunately the way it is done
 * in Nuke
 *
 * But either way, it's not too important
 */
[ExcludeFromCodeCoverage]
internal sealed class NukeLogger : ILogger
{
    public void Log(LogLevel level, string message, Exception? exception = null)
    {
        var logEventLevel = ToLogEventLevel(level);
        
        // ReSharper disable once TemplateIsNotCompileTimeConstantProblem
        Serilog.Log.Write(logEventLevel, exception, message);
    }

    private static LogEventLevel ToLogEventLevel(LogLevel logLevel)
    {
        return logLevel switch
        {
            LogLevel.Trace => LogEventLevel.Verbose,
            LogLevel.Debug => LogEventLevel.Debug,
            LogLevel.Information => LogEventLevel.Information,
            LogLevel.Warning => LogEventLevel.Warning,
            LogLevel.Error => LogEventLevel.Error,
            LogLevel.Critical => LogEventLevel.Fatal,
            LogLevel.None => LogEventLevel.Fatal,
            _ => throw new ArgumentOutOfRangeException(nameof(logLevel), logLevel, null)
        };
    }
}
