using Cake.Core;
using Cake.Core.Diagnostics;
using Liz.Core.Logging.Contracts;
using LogLevel = Liz.Core.Logging.Contracts.LogLevel;
using CakeLogLevel = Cake.Core.Diagnostics.LogLevel;

namespace Liz.Cake.Logging;

internal sealed class CakeLogger : ILogger
{
    private readonly ICakeContext _cakeContext;

    public CakeLogger(ICakeContext cakeContext)
    {
        _cakeContext = cakeContext ?? throw new ArgumentNullException(nameof(cakeContext));
    }
    
    public void Log(LogLevel level, string message, Exception? exception = null)
    {
        var verbosity = LogLevelToVerbosity(level);
        var cakeLogLevel = LogLevelToCakeLogLevel(level);

        var actualMessage = exception == null 
            ? message 
            : $"{message}{Environment.NewLine}{Environment.NewLine}{exception}";

        actualMessage = actualMessage.Replace("{", "{{");
        actualMessage = actualMessage.Replace("}", "}}");
        
        _cakeContext.Log.Write(verbosity, cakeLogLevel, actualMessage);
    }

    private static Verbosity LogLevelToVerbosity(LogLevel logLevel)
    {
        return logLevel switch
        {
            LogLevel.Trace => Verbosity.Diagnostic,
            LogLevel.Debug => Verbosity.Verbose,
            LogLevel.Information => Verbosity.Normal,
            LogLevel.Warning => Verbosity.Normal,
            LogLevel.Error => Verbosity.Normal,
            LogLevel.Critical => Verbosity.Normal,
            LogLevel.None => Verbosity.Quiet,
            _ => throw new ArgumentOutOfRangeException(nameof(logLevel), logLevel, null)
        };
    }

    private static CakeLogLevel LogLevelToCakeLogLevel(LogLevel logLevel)
    {
        return logLevel switch
        {
            LogLevel.Trace => CakeLogLevel.Debug,
            LogLevel.Debug => CakeLogLevel.Verbose,
            LogLevel.Information => CakeLogLevel.Information,
            LogLevel.Warning => CakeLogLevel.Warning,
            LogLevel.Error => CakeLogLevel.Error,
            LogLevel.Critical => CakeLogLevel.Error,
            LogLevel.None => CakeLogLevel.Information,
            _ => throw new ArgumentOutOfRangeException(nameof(logLevel), logLevel, null)
        };
    }
}
