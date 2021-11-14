using DotnetNugetLicenses.Core.Logging;
using System;
using System.Diagnostics.CodeAnalysis;

namespace DotnetNugetLicenses.Tool.Logging;

[ExcludeFromCodeCoverage] // not too sure how to test this properly
internal sealed class CommandLineLogger : ILogger
{
    private readonly LogLevel _configuredLogLevel;

    public CommandLineLogger(LogLevel configuredLogLevel)
    {
        _configuredLogLevel = configuredLogLevel;
    }
    
    public void Log(LogLevel level, string message, Exception exception = null)
    {
        if (!IsEnabled(level)) return;

        var color = GetConsoleColorFromLevel(level);
        
        var text = exception != null ? $"{message}\n{exception}" : message;
        WriteLine(text, color);
    }

    public void LogTrace(string message, Exception exception = null)
    {
        Log(LogLevel.Trace, message, exception);
    }

    public void LogDebug(string message, Exception exception = null)
    {
        Log(LogLevel.Debug, message, exception);
    }

    public void LogInformation(string message, Exception exception = null)
    {
        Log(LogLevel.Information, message, exception);
}

    public void LogWarning(string message, Exception exception = null)
    {
        Log(LogLevel.Warning, message, exception);
    }

    public void LogError(string message, Exception exception = null)
    {
        Log(LogLevel.Error, message, exception);
    }

    public void LogCritical(string message, Exception exception = null)
    {
        Log(LogLevel.Critical, message, exception);
    }

    private bool IsEnabled(LogLevel logLevel)
    {
        return (int)logLevel >= (int)_configuredLogLevel;
    }
    
    private static ConsoleColor? GetConsoleColorFromLevel(LogLevel level)
    {
        ConsoleColor? color = level switch
        {
            LogLevel.Trace => ConsoleColor.DarkGray, // 'Gray' does not work in CMD
            LogLevel.Debug => ConsoleColor.DarkGray, // 'Gray' does not work in CMD
            LogLevel.Information => null,
            LogLevel.Warning => ConsoleColor.Yellow, // 'DarkYellow' does not work in PowerShell
            LogLevel.Error => ConsoleColor.Red,
            LogLevel.Critical => ConsoleColor.DarkRed,
            LogLevel.None => null,
            _ => throw new ArgumentOutOfRangeException(nameof(level), level, null)
        };

        return color;
    }

    private static void WriteLine(string text, ConsoleColor? color = null)
    {
        if (!color.HasValue)
            Console.WriteLine(text);
        else
            WriteLineWithColor(text, color.Value);
    }

    private static void WriteLineWithColor(string text, ConsoleColor color)
    {
        var previousColor = Console.ForegroundColor;

        Console.ForegroundColor = color;
        Console.WriteLine(text);
        Console.ForegroundColor = previousColor;
    }
}
