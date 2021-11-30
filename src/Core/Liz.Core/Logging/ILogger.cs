using JetBrains.Annotations;
using System;

namespace DotnetNugetLicenses.Core.Logging;

[PublicAPI]
public interface ILogger
{
    public void Log(LogLevel level, string message, Exception exception = null);
}
