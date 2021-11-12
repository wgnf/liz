using ArrangeContext.Moq;
using DotnetNugetLicenses.Core.Contracts;
using DotnetNugetLicenses.Tool.CommandLine;
using DotnetNugetLicenses.Tool.Contracts.CommandLine;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.IO;
using System.IO.Abstractions;
using Unity;
using Xunit;

namespace DotnetNugetLicenses.Tool.Tests.CommandLine
{
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
            var extractLicensesFactory = new Func<IExtractLicenses>(() => extractLicenses.Object);
            var fileSystem = new Mock<IFileSystem>();
            var unityContainer = new Mock<IUnityContainer>();

            var sut = new CommandRunner(extractLicensesFactory, fileSystem.Object, unityContainer.Object);

            var fileInfo = new FileInfo("some/file.txt");

            sut.Run(fileInfo, LogLevel.Information);

            extractLicenses
                .Verify(e => e.Extract(It.Is<ExtractSettings>(settings =>
                    settings.TargetFile.FullName == fileInfo.FullName)), Times.Once());
        }
    }
}
