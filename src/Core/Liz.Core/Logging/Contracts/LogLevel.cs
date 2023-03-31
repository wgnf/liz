namespace Liz.Core.Logging.Contracts;

/// <summary>
///     The log level
/// </summary>
public enum LogLevel
{
    /// <summary>
    ///     most verbose level which includes basically everything
    /// </summary>
    Trace = 0,

    /// <summary>
    ///     debug level which includes debug messages
    /// </summary>
    Debug = 1,

    /// <summary>
    ///     information level which includes basic messages - this is generally the default
    /// </summary>
    Information = 2,

    /// <summary>
    ///     warning level which includes warnings for the user
    /// </summary>
    Warning = 3,

    /// <summary>
    ///     error level which includes errors for the user
    /// </summary>
    Error = 4,

    /// <summary>
    ///     critical level which includes most critical errors for the user
    /// </summary>
    Critical = 5,

    /// <summary>
    ///     does not log anything
    /// </summary>
    None = 6
}
