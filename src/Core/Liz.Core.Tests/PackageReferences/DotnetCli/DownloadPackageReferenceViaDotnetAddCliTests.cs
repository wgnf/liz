using ArrangeContext.Moq;
using FluentAssertions;
using Liz.Core.CliTool;
using Liz.Core.Logging;
using Liz.Core.PackageReferences;
using Liz.Core.PackageReferences.DotnetCli;
using Liz.Core.Utils;
using Moq;
using System;
using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;
using System.Threading.Tasks;
using Xunit;

namespace Liz.Core.Tests.PackageReferences.DotnetCli;

public class DownloadPackageReferenceViaDotnetAddCliTests
{
    [Fact]
    public async Task Download_Should_Throw_On_Invalid_Parameter()
    {
        var context = new ArrangeContext<DownloadPackageReferenceViaDotnetAddCli>();
        var sut = context.Build();

        await Assert.ThrowsAsync<ArgumentNullException>(() => sut.DownloadAsync(null!));
    }

    [Fact]
    public async Task Download()
    {
        var mockFileSystem = new MockFileSystem();
        
        var context = new ArrangeContext<DownloadPackageReferenceViaDotnetAddCli>();
        context.Use<IFileSystem>(mockFileSystem);

        const string tempDirectory = "C:\\tmp";
        context
            .For<IProvideTemporaryDirectory>()
            .Setup(provider => provider.Get())
            .Returns(new MockDirectoryInfo(mockFileSystem, tempDirectory));
        
        var sut = context.Build();

        var packageReference = new PackageReference("Something", "net5.0", "1337");
        var result = await sut.DownloadAsync(packageReference);

        mockFileSystem
            .AllDirectories
            .Should()
            .Contain(directory => 
                directory.Contains(tempDirectory));

        mockFileSystem
            .GetFile($"{tempDirectory}\\download\\Dummy.csproj")
            .TextContents
            .Should()
            .ContainAll(
                "Project Sdk",
                "TargetFramework",
                packageReference.TargetFramework);

        result
            .FullName
            .Should()
            .Be($"{tempDirectory}\\download\\{packageReference.Name.ToLower()}\\{packageReference.Version.ToLower()}");
    }

    [Fact]
    public async Task Download_Should_Not_Fail_When_Cli_Execution_Failed()
    {
        var mockFileSystem = new MockFileSystem();
        
        var context = new ArrangeContext<DownloadPackageReferenceViaDotnetAddCli>();
        context.Use<IFileSystem>(mockFileSystem);

        const string tempDirectory = "C:\\tmp";
        context
            .For<IProvideTemporaryDirectory>()
            .Setup(provider => provider.Get())
            .Returns(new MockDirectoryInfo(mockFileSystem, tempDirectory));
        
        // !!!
        context
            .For<ICliToolExecutor>()
            .Setup(executor => executor.ExecuteAsync(It.IsAny<string>(), It.IsAny<string>()))
            .Throws<Exception>();
        
        var sut = context.Build();

        var packageReference = new PackageReference("Something", "net5.0", "1337");
        _ = await sut.DownloadAsync(packageReference);

        context
            .For<ILogger>()
            .Verify(logger =>
                    logger.Log(
                        LogLevel.Debug, 
                        It.Is<string>(message => message.Contains("could not be downloaded")), 
                        It.IsAny<Exception>()),
                Times.Once);
    }
}
