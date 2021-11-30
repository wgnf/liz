﻿using System;
using System.Diagnostics.CodeAnalysis;

namespace DotnetNugetLicenses.Core.Logging.Null;

[ExcludeFromCodeCoverage] // does nothing
internal sealed class NullLogger : ILogger
{
    public void Log(LogLevel level, string message, Exception exception = null)
    {
        // does nothing
    }
}