﻿using System.Diagnostics.CodeAnalysis;

namespace DotnetNugetLicenses.Core.Logging.Null;

[ExcludeFromCodeCoverage] // provides null-logger which does nothing
internal sealed class NullLoggerProvider : ILoggerProvider
{
    public ILogger Get(LogLevel logLevel)
    {
        return new NullLogger();
    }
}