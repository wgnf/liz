using JetBrains.Annotations;

namespace Liz.Core.Logging.Contracts;

[PublicAPI]
public interface ILoggerProvider
{
    ILogger Get(LogLevel logLevel);
}
