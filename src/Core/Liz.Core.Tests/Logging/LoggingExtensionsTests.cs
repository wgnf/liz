using Liz.Core.Logging;
using Liz.Core.Logging.Contracts;
using Moq;
using Xunit;

namespace Liz.Core.Tests.Logging;

public class LoggingExtensionsTests
{
    [Fact]
    public void Log_Trace_Should_Fail_When_Logger_Null()
    {
        Assert.Throws<ArgumentNullException>(() => ((ILogger)null!).LogTrace("message"));
    }

    [Fact]
    public void Log_Trace()
    {
        var loggerMock = new Mock<ILogger>();

        const string message = "message";
        var exception = new Exception();
        
        loggerMock
            .Object
            .LogTrace(message, exception);
        
        loggerMock
            .Verify(logger => logger.Log(LogLevel.Trace, message, exception), Times.Once);
    }
    
    [Fact]
    public void Log_Debug_Should_Fail_When_Logger_Null()
    {
        Assert.Throws<ArgumentNullException>(() => ((ILogger)null!).LogDebug("message"));
    }

    [Fact]
    public void Log_Debug()
    {
        var loggerMock = new Mock<ILogger>();

        const string message = "message";
        var exception = new Exception();
        
        loggerMock
            .Object
            .LogDebug(message, exception);
        
        loggerMock
            .Verify(logger => logger.Log(LogLevel.Debug, message, exception), Times.Once);
    }
    
    [Fact]
    public void Log_Information_Should_Fail_When_Logger_Null()
    {
        Assert.Throws<ArgumentNullException>(() => ((ILogger)null!).LogInformation("message"));
    }

    [Fact]
    public void Log_Information()
    {
        var loggerMock = new Mock<ILogger>();

        const string message = "message";
        var exception = new Exception();
        
        loggerMock
            .Object
            .LogInformation(message, exception);
        
        loggerMock
            .Verify(logger => logger.Log(LogLevel.Information, message, exception), Times.Once);
    }
    
    [Fact]
    public void Log_Warning_Should_Fail_When_Logger_Null()
    {
        Assert.Throws<ArgumentNullException>(() => ((ILogger)null!).LogWarning("message"));
    }

    [Fact]
    public void Log_Warning()
    {
        var loggerMock = new Mock<ILogger>();

        const string message = "message";
        var exception = new Exception();
        
        loggerMock
            .Object
            .LogWarning(message, exception);
        
        loggerMock
            .Verify(logger => logger.Log(LogLevel.Warning, message, exception), Times.Once);
    }
    
    [Fact]
    public void Log_Error_Should_Fail_When_Logger_Null()
    {
        Assert.Throws<ArgumentNullException>(() => ((ILogger)null!).LogError("message"));
    }

    [Fact]
    public void Log_Error()
    {
        var loggerMock = new Mock<ILogger>();

        const string message = "message";
        var exception = new Exception();
        
        loggerMock
            .Object
            .LogError(message, exception);
        
        loggerMock
            .Verify(logger => logger.Log(LogLevel.Error, message, exception), Times.Once);
    }
    
    [Fact]
    public void Log_Critical_Should_Fail_When_Logger_Null()
    {
        Assert.Throws<ArgumentNullException>(() => ((ILogger)null!).LogCritical("message"));
    }

    [Fact]
    public void Log_Critical()
    {
        var loggerMock = new Mock<ILogger>();

        const string message = "message";
        var exception = new Exception();
        
        loggerMock
            .Object
            .LogCritical(message, exception);
        
        loggerMock
            .Verify(logger => logger.Log(LogLevel.Critical, message, exception), Times.Once);
    }
}
