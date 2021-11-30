using JetBrains.Annotations;
using System;

namespace Liz.Core.Logging;

[PublicAPI]
public interface ILogger
{
    public void Log(LogLevel level, string message, Exception exception = null);
}
