using ArrangeContext.Moq;
using Cake.Core;
using Cake.Core.Diagnostics;
using Liz.Cake.Logging;
using Moq;
using Xunit;
using LogLevel = Liz.Core.Logging.Contracts.LogLevel;
using CakeLogLevel = Cake.Core.Diagnostics.LogLevel;

namespace Liz.Cake.Tests.Logging;

public class CakeLoggerTests
{
    [Theory]
    [InlineData(LogLevel.Trace, Verbosity.Diagnostic, CakeLogLevel.Debug)]
    [InlineData(LogLevel.Debug, Verbosity.Verbose, CakeLogLevel.Verbose)]
    [InlineData(LogLevel.Information, Verbosity.Normal, CakeLogLevel.Information)]
    [InlineData(LogLevel.Warning, Verbosity.Normal, CakeLogLevel.Warning)]
    [InlineData(LogLevel.Error, Verbosity.Normal, CakeLogLevel.Error)]
    [InlineData(LogLevel.Critical, Verbosity.Normal, CakeLogLevel.Error)]
    [InlineData(LogLevel.None, Verbosity.Quiet, CakeLogLevel.Information)]
    public void CakeLogger_Logs_With_Correctly_Mapped_Cake_Values(
        LogLevel lizLogLevel,
        Verbosity expectedCakeVerbosity,
        CakeLogLevel expectedCakeLogLevel)
    {
        var context = ArrangeContext<CakeLogger>.Create();
        var sut = context.Build();

        var cakeLogMock = new Mock<ICakeLog>();
        context
            .For<ICakeContext>()
            .SetupGet(cakeContext => cakeContext.Log)
            .Returns(cakeLogMock.Object);

        const string message = "something";
        
        sut.Log(lizLogLevel, message);
        
        cakeLogMock.Verify(cakeLog => 
            cakeLog.Write(expectedCakeVerbosity, expectedCakeLogLevel, message));
    }
}
