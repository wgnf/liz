using JetBrains.Annotations;

namespace Liz.Core.Logging;

[PublicAPI]
public interface ILoggerProvider
{
    ILogger Get(LogLevel logLevel);
}
