using System;
using System.Diagnostics.CodeAnalysis;

namespace Liz.Core.CliTool.Contracts.Exceptions;

[ExcludeFromCodeCoverage] // exception
internal sealed class CliToolExecutionFailedException : Exception
{
    public CliToolExecutionFailedException(
        string fileName,
        string arguments,
        string reason,
        Exception inner = null)
        : base($"Execution of '{fileName} {arguments}' failed, because: {reason}", inner)
    {
    }

    internal CliToolExecutionFailedException(
        string fileName,
        string arguments,
        int exitCode,
        string errorOutput)
        : this(fileName, arguments, $"Exit code {exitCode} does not indicate success. Error-Output:\n{errorOutput}")
    {
    }
}
