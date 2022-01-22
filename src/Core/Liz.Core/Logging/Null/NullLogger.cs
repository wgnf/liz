using Liz.Core.Logging.Contracts;
using System;
using System.Diagnostics.CodeAnalysis;

namespace Liz.Core.Logging.Null;

[ExcludeFromCodeCoverage] // does nothing
internal sealed class NullLogger : ILogger
{
    public void Log(LogLevel level, string message, Exception exception = null)
    {
        // does nothing
    }
}
