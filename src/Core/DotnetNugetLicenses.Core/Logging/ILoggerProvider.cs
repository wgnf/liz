using JetBrains.Annotations;

namespace DotnetNugetLicenses.Core.Logging;

[PublicAPI]
public interface ILoggerProvider
{
    ILogger Get(LogLevel logLevel);
}
