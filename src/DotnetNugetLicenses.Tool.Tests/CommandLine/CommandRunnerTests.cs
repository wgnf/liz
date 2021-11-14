using ArrangeContext.Moq;
using DotnetNugetLicenses.Core;
using DotnetNugetLicenses.Core.Extract;
using DotnetNugetLicenses.Core.Logging;
using DotnetNugetLicenses.Core.Settings;
using DotnetNugetLicenses.Tool.CommandLine;
using DotnetNugetLicenses.Tool.Contracts.CommandLine;
using FluentAssertions;
using Moq;
using System;
using System.IO;
using Xunit;

namespace DotnetNugetLicenses.Tool.Tests.CommandLine;

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
    public void Should_Fail_To_Run_When_Invalid_Parameters()
    {
        var sut = new ArrangeContext<CommandRunner>().Build();

        Assert.Throws<ArgumentNullException>(() => sut.Run(null, LogLevel.Information));
    }

    [Fact]
    public void Should_Forward_Execution_To_Extract_Licenses_With_Settings()
    {
        var extractLicenses = new Mock<IExtractLicenses>();
        var extractLicensesFactory = new Mock<IExtractLicensesFactory>();
        extractLicensesFactory
            .Setup(factory => factory.Create(It.IsAny<ExtractLicensesSettings>(), It.IsAny<ILoggerProvider>()))
            .Returns(extractLicenses.Object);

        var sut = new CommandRunner(extractLicensesFactory.Object);

        var fileInfo = new FileInfo("some/file.txt");

        sut.Run(fileInfo, LogLevel.Information);

        extractLicenses.Verify(e => e.Extract(), Times.Once());
    }
}
