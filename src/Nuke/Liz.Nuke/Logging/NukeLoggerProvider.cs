using System.Diagnostics.CodeAnalysis;
using Liz.Core.Logging.Contracts;

namespace Liz.Nuke.Logging;

[ExcludeFromCodeCoverage] // simple provider
internal sealed class NukeLoggerProvider : ILoggerProvider
{
    /// <inheritdoc />
    public ILogger Get(LogLevel logLevel)
    {
        return new NukeLogger();
    }
}
