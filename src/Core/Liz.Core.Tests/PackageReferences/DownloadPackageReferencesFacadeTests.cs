﻿using ArrangeContext.Moq;
using FluentAssertions;
using Liz.Core.Logging.Contracts;
using Liz.Core.PackageReferences;
using Liz.Core.PackageReferences.Contracts.DotnetCli;
using Liz.Core.PackageReferences.Contracts.Models;
using Liz.Core.Preparation.Contracts.Models;
using Liz.Core.Utils.Contracts;
using Moq;
using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;
using Xunit;

namespace Liz.Core.Tests.PackageReferences;

public class DownloadPackageReferencesFacadeTests
{
    [Fact]
    public async Task DownloadAndEnrich_Fails_On_Invalid_Parameters()
    {
        var context = CreateContext();
        var sut = context.Build();

        await Assert.ThrowsAsync<ArgumentNullException>(() => sut.DownloadAndEnrichAsync(null!));
    }

    [Fact]
    public async Task DownloadAndEnrich_Does_Nothing_When_There_Are_No_Packages()
    {
        var mockFileSystem = new MockFileSystem();
        
        var context = CreateContext();
        context.Use<IFileSystem>(mockFileSystem);
        
        var sut = context.Build();
        
        const string downloadDirectory = "C:/temp/download";
        var downloadDirectoryMock = new Mock<IDirectoryInfo>();
        downloadDirectoryMock
            .SetupGet(directory => directory.FullName)
            .Returns(downloadDirectory);
        
        context
            .For<IProvideTemporaryDirectories>()
            .Setup(provideTempDirectories => provideTempDirectories.GetDownloadDirectory())
            .Returns(downloadDirectoryMock.Object);

        var packageReferences = Enumerable.Empty<PackageReference>();
        await sut.DownloadAndEnrichAsync(packageReferences);

        context
            .For<IDownloadPackageReferencesViaDotnetCli>()
            .Verify(downloader => downloader.DownloadAsync(It.IsAny<IFileInfo>(), It.IsAny<IDirectoryInfo>()),
                Times.Never);
    }

    [Fact]
    public async Task DownloadAndEnrich_Creates_Download_Directory()
    {
        var mockFileSystem = new MockFileSystem();
        
        var context = CreateContext();
        context.Use<IFileSystem>(mockFileSystem);
        
        var sut = context.Build();
        
        const string downloadDirectory = "C:/temp/download";
        var downloadDirectoryMock = new Mock<IDirectoryInfo>();
        downloadDirectoryMock
            .SetupGet(directory => directory.FullName)
            .Returns(downloadDirectory);
        
        context
            .For<IProvideTemporaryDirectories>()
            .Setup(provideTempDirectories => provideTempDirectories.GetDownloadDirectory())
            .Returns(downloadDirectoryMock.Object);

        var packageReferences = new[] { new PackageReference("Test", "net6.0", "1.2.3") };
        await sut.DownloadAndEnrichAsync(packageReferences);

        mockFileSystem
            .AllDirectories
            .Should()
            .NotBeEmpty();
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task DownloadAndEnrich(bool isCpmEnabled)
    {
        var mockFileSystem = new MockFileSystem();

        var context = ArrangeContext<DownloadPackageReferencesFacade>.Create();
        context.Use(new SourceInfo { IsCpmEnabled = isCpmEnabled });
        context.Use<IFileSystem>(mockFileSystem);

        const string downloadDirectory = "C:/temp/download";
        var downloadDirectoryMock = new Mock<IDirectoryInfo>();
        downloadDirectoryMock
            .SetupGet(directory => directory.FullName)
            .Returns(downloadDirectory);
        
        context
            .For<IProvideTemporaryDirectories>()
            .Setup(provideTempDirectories => provideTempDirectories.GetDownloadDirectory())
            .Returns(downloadDirectoryMock.Object);
        
        var sut = context.Build();

        var packageReferences = new[]
        {
            new PackageReference("TestNet6.0-1", "net6.0", "1.2.3"),
            new PackageReference("TestNet6.0-2", "net6.0", "1.2.5"),
            new PackageReference("TestNetStandard", "netstandard2.0", "1.3.3.7")
        };

        // ReSharper disable once StringLiteralTypo
        var netStandardPackageArtifactDirectory = mockFileSystem.Path.Combine(downloadDirectory, "testnetstandard", "1.3.3.7");
        
        // just pretend we have on artifact-directory
        mockFileSystem.AddDirectory(netStandardPackageArtifactDirectory);

        await sut.DownloadAndEnrichAsync(packageReferences);

        // twice, because there are exactly two different target-frameworks declared
        context
            .For<IDownloadPackageReferencesViaDotnetCli>()
            .Verify(downloader => downloader.DownloadAsync(It.IsAny<IFileInfo>(), It.IsAny<IDirectoryInfo>()),
                Times.Exactly(2));
        
        // according to above there should be at least one item with the artifact-directory set
        packageReferences
            .Should()
            .Contain(packageReference => packageReference.ArtifactDirectory != null);
    }
    
    [Fact]
    public async Task DownloadAndEnrich_Handles_Exceptions()
    {
        var mockFileSystem = new MockFileSystem();

        var context = CreateContext();
        context.Use<IFileSystem>(mockFileSystem);

        const string downloadDirectory = "C:/temp/download";
        var downloadDirectoryMock = new Mock<IDirectoryInfo>();
        downloadDirectoryMock
            .SetupGet(directory => directory.FullName)
            .Returns(downloadDirectory);
        
        context
            .For<IProvideTemporaryDirectories>()
            .Setup(provideTempDirectories => provideTempDirectories.GetDownloadDirectory())
            .Returns(downloadDirectoryMock.Object);
        
        var sut = context.Build();

        var packageReferences = new[]
        {
            new PackageReference("TestNet6.0-1", "net6.0", "1.2.3"),
            new PackageReference("TestNet6.0-2", "net6.0", "1.2.5"),
            new PackageReference("TestNetStandard", "netstandard2.0", "1.3.3.7")
        };

        // ReSharper disable once StringLiteralTypo
        var netStandardPackageArtifactDirectory = mockFileSystem.Path.Combine(downloadDirectory, "testnetstandard", "1.3.3.7");
        
        // just pretend we have on artifact-directory
        mockFileSystem.AddDirectory(netStandardPackageArtifactDirectory);
        
        // twice, because there are exactly two different target-frameworks declared
        context
            .For<IDownloadPackageReferencesViaDotnetCli>()
            .Setup(downloader => downloader.DownloadAsync(It.IsAny<IFileInfo>(), It.IsAny<IDirectoryInfo>()))
            .Throws<InvalidOperationException>();

        await sut.DownloadAndEnrichAsync(packageReferences);

        context
            .For<ILogger>()
            .Verify(logger => logger.Log(
                LogLevel.Debug, 
                It.Is<string>(message => message.Contains("Unable to download")), 
                It.Is<Exception>(exception => exception != null)), 
                Times.AtLeast(1));
    }

    private static ArrangeContext<DownloadPackageReferencesFacade> CreateContext()
    {
        var context = ArrangeContext<DownloadPackageReferencesFacade>.Create();
        
        context.Use(new SourceInfo());

        return context;
    }
}
