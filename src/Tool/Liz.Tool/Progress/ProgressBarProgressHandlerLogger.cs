using Liz.Core.Logging.Contracts;
using Liz.Core.Progress;
using Liz.Tool.Logging;
using ShellProgressBar;
using System.Diagnostics.CodeAnalysis;

namespace Liz.Tool.Progress;

[ExcludeFromCodeCoverage] // not sure how to test this properly
internal sealed class ProgressBarProgressHandlerLogger : IProgressHandler, ILogger
{
    private readonly LogLevel _configuredLogLevel;
    private readonly CommandLineLoggerProvider _commandLineLoggerProvider;
    private readonly List<IDisposable> _disposables = new();

    private ILogger? _commandLineLogger;
    private IProgressBar? _mainProgressBar;
    private IProgressBar? _currentSubProgressBar;

    private bool _isFinished;

    public ProgressBarProgressHandlerLogger(LogLevel configuredLogLevel, CommandLineLoggerProvider commandLineLoggerProvider)
    {
        _configuredLogLevel = configuredLogLevel;
        _commandLineLoggerProvider = commandLineLoggerProvider ?? throw new ArgumentNullException(nameof(commandLineLoggerProvider));
    }
    
    public void InitializeMainProcess(int totalSteps, string initialMessage)
    {
        var mainProgressBarOptions = new ProgressBarOptions
        {
            CollapseWhenFinished = false
        };

        _mainProgressBar = new ProgressBar(totalSteps, initialMessage, mainProgressBarOptions);
        _disposables.Add(_mainProgressBar);
    }

    public void TickMainProcess(string message)
    {
        _mainProgressBar?.Tick(message);
    }

    public void FinishMainProcess()
    {
        // the last tick so it is finished
        _mainProgressBar?.Tick();
        _isFinished = true;
        
        _disposables.ForEach(disposable => disposable.Dispose());
    }

    public void InitializeNewSubProcess(int totalSteps)
    {
        var subProgressBarOptions = new ProgressBarOptions
        {
            CollapseWhenFinished = false
        };

        _currentSubProgressBar = _mainProgressBar?.Spawn(totalSteps, string.Empty, subProgressBarOptions);
        
        if (_currentSubProgressBar != null)
            _disposables.Add(_currentSubProgressBar);
    }

    public void TickCurrentSubProcess(string message)
    {
        _currentSubProgressBar?.Tick(message);
    }
    
    public void Log(LogLevel level, string message, Exception? exception = null)
    {
        if (!IsEnabled(level)) return;

        switch (_isFinished)
        {
            case false when (int)level >= (int)LogLevel.Warning:
                _mainProgressBar?.WriteErrorLine($"{message}{(exception == null ? "" : $": {exception.Message}")}");
                return;
            case false:
                return;
            
            default:
                _commandLineLogger ??= _commandLineLoggerProvider.Get(_configuredLogLevel);
                _commandLineLogger.Log(level, message, exception);
                break;
        }
    }
    
    private bool IsEnabled(LogLevel logLevel)
    {
        return (int)logLevel >= (int)_configuredLogLevel;
    }
}
