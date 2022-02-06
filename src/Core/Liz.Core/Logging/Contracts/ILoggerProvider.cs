namespace Liz.Core.Logging.Contracts;

public interface ILoggerProvider
{
    ILogger Get(LogLevel logLevel);
}
