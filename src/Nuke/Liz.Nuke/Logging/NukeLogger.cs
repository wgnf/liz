using Liz.Core.Logging.Contracts;
using Serilog.Events;
using ILogger = Liz.Core.Logging.Contracts.ILogger;

namespace Liz.Nuke.Logging;

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
