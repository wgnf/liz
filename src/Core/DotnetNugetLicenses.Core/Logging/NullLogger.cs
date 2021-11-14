using System;
using System.Diagnostics.CodeAnalysis;

namespace DotnetNugetLicenses.Core.Logging;

[ExcludeFromCodeCoverage] // does nothing
internal sealed class NullLogger : ILogger
{
    public void Log(LogLevel level, string message, Exception exception = null)
    {
        // does nothing
    }

    public void LogTrace(string message, Exception exception = null)
    {
        // does nothing
    }

    public void LogDebug(string message, Exception exception = null)
    {
        // does nothing
    }

    public void LogInformation(string message, Exception exception = null)
    {
        // does nothing
    }

    public void LogWarning(string message, Exception exception = null)
    {
        // does nothing
    }

    public void LogError(string message, Exception exception = null)
    {
        // does nothing
    }

    public void LogCritical(string message, Exception exception = null)
    {
        // does nothing
    }
}
