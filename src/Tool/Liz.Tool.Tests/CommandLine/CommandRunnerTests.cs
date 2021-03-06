using ArrangeContext.Moq;
using FluentAssertions;
using Liz.Core;
using Liz.Core.Extract.Contracts;
using Liz.Core.Logging.Contracts;
using Liz.Core.Progress;
using Liz.Core.Settings;
using Liz.Tool.CommandLine;
using Liz.Tool.Contracts.CommandLine;
using Moq;
using Xunit;

namespace Liz.Tool.Tests.CommandLine;

public sealed class CommandRunnerTests
{
    [Fact]
    public void Should_Have_Correct_Interface()
    {
        var sut = new ArrangeContext<CommandRunner>().Build();

        sut
            .Should()
            .BeAssignableTo<ICommandRunner>();
    }

    [Fact]
    public async Task Should_Fail_To_Run_When_Invalid_Parameters()
    {
        var sut = new ArrangeContext<CommandRunner>().Build();

        await Assert.ThrowsAsync<ArgumentNullException>(() => sut.RunAsync(
            null!, 
            LogLevel.Information,
            true, 
            true, 
            true,
            true,
            null,
            null,
            null,
            null,
            null));
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task Should_Forward_Execution_To_Extract_Licenses_With_Settings(bool booleanParameter)
    {
        var extractLicenses = new Mock<IExtractLicenses>();
        var extractLicensesFactory = new Mock<IExtractLicensesFactory>();
        
        extractLicensesFactory
            .Setup(factory => factory.Create(
                It.IsAny<ExtractLicensesSettingsBase>(), 
                It.IsAny<ILoggerProvider>(),
                It.IsAny<LogLevel>(),
                It.IsAny<IProgressHandler?>()))
            .Returns(extractLicenses.Object);

        var sut = new CommandRunner(extractLicensesFactory.Object);

        var fileInfo = new FileInfo("some/file.txt");

        await sut.RunAsync(
            fileInfo, 
            LogLevel.Information,
            booleanParameter, 
            booleanParameter, 
            booleanParameter,
            booleanParameter,
            null,
            null,
            null,
            null,
            null);

        extractLicenses.Verify(e => e.ExtractAsync(), Times.Once());
    }
}
