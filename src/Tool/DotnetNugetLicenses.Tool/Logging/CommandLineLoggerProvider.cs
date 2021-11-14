using DotnetNugetLicenses.Core.Logging;
using System.Diagnostics.CodeAnalysis;

namespace DotnetNugetLicenses.Tool.Logging;

[ExcludeFromCodeCoverage] // simple provider
internal sealed class CommandLineLoggerProvider : ILoggerProvider
{
    public ILogger Get(LogLevel logLevel)
    {
        return new CommandLineLogger(logLevel);
    }
}
