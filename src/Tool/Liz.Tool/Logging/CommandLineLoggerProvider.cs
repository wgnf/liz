using Liz.Core.Logging.Contracts;
using System.Diagnostics.CodeAnalysis;

namespace Liz.Tool.Logging;

[ExcludeFromCodeCoverage] // simple provider
internal sealed class CommandLineLoggerProvider : ILoggerProvider
{
    public ILogger Get(LogLevel logLevel)
    {
        return new CommandLineLogger(logLevel);
    }
}
