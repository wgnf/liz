﻿using JetBrains.Annotations;

namespace Liz.Core.Logging;

[PublicAPI]
public enum LogLevel
{
    Trace = 0,
    Debug = 1,
    Information = 2,
    Warning = 3,
    Error = 4,
    Critical = 5,
    None = 6
}
