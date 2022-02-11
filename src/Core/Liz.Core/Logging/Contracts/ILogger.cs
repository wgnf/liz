namespace Liz.Core.Logging.Contracts;

public interface ILogger
{
    public void Log(LogLevel level, string message, Exception? exception = null);
}
