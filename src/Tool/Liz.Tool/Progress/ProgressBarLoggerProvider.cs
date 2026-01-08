using System.Diagnostics.CodeAnalysis;
using Liz.Core.Logging.Contracts;
using Liz.Tool.Logging;

namespace Liz.Tool.Progress;

[ExcludeFromCodeCoverage] // simple factory
internal sealed class ProgressBarLoggerProvider : ILoggerProvider
{
    private readonly CommandLineLoggerProvider _commandLineLoggerProvider;
    private ProgressBarProgressHandlerLogger? _instance;

    public ProgressBarLoggerProvider(CommandLineLoggerProvider commandLineLoggerProvider)
    {
        _commandLineLoggerProvider = commandLineLoggerProvider ?? throw new ArgumentNullException(nameof(commandLineLoggerProvider));
    }

    public ILogger Get(LogLevel logLevel)
    {
        _instance ??= new ProgressBarProgressHandlerLogger(logLevel, _commandLineLoggerProvider);
        return _instance;
    }
}
