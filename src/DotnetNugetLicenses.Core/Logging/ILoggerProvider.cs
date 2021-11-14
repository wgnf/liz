namespace DotnetNugetLicenses.Core.Logging;

public interface ILoggerProvider
{
    ILogger Get(LogLevel logLevel);
}
