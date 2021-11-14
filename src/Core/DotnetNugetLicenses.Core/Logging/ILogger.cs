using JetBrains.Annotations;
using System;

namespace DotnetNugetLicenses.Core.Logging;

[PublicAPI]
public interface ILogger
{
    public void Log(LogLevel level, string message, Exception exception = null);
    public void LogTrace(string message, Exception exception = null);
    public void LogDebug(string message, Exception exception = null);
    public void LogInformation(string message, Exception exception = null);
    public void LogWarning(string message, Exception exception = null);
    public void LogError(string message, Exception exception = null);
    public void LogCritical(string message, Exception exception = null);
}
