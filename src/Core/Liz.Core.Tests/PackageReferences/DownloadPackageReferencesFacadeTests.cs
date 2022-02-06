using ArrangeContext.Moq;
using Liz.Core.PackageReferences;
using Liz.Core.PackageReferences.Contracts.DotnetCli;
using Liz.Core.Projects.Contracts.Models;
using Liz.Core.Utils.Contracts;
using Moq;
using System;
using System.IO.Abstractions;
using System.Threading.Tasks;
using Xunit;

namespace Liz.Core.Tests.PackageReferences;

public class DownloadPackageReferencesFacadeTests
{
    [Fact]
    public async Task DownloadForProject_Fails_On_Invalid_Parameters()
    {
        var context = ArrangeContext<DownloadPackageReferencesFacade>.Create();
        var sut = context.Build();

        await Assert.ThrowsAsync<ArgumentNullException>(() => sut.DownloadForProjectAsync(null!));
    }

    [Fact]
    public async Task DownloadForProject_Creates_Download_Directory()
    {
        var context = ArrangeContext<DownloadPackageReferencesFacade>.Create();
        var sut = context.Build();

        var downloadDirectory = new Mock<IDirectoryInfo>();
        context
            .For<IProvideTemporaryDirectories>()
            .Setup(provideTempDirectories => provideTempDirectories.GetDownloadDirectory())
            .Returns(downloadDirectory.Object);

        await sut.DownloadForProjectAsync(new Project("something", Mock.Of<IFileInfo>(), ProjectFormatStyle.Unknown));

        downloadDirectory
            .Verify(directory => directory.Create(), Times.Once);
    }

    [Fact]
    public async Task DownloadForProject_Uses_Dotnet_Cli_For_Sdk_Style_Project()
    {
        var context = ArrangeContext<DownloadPackageReferencesFacade>.Create();
        var sut = context.Build();

        context
            .For<IProvideTemporaryDirectories>()
            .Setup(provideTempDirectories => provideTempDirectories.GetDownloadDirectory())
            .Returns(Mock.Of<IDirectoryInfo>());

        var project = new Project("SomeProject", Mock.Of<IFileInfo>(), ProjectFormatStyle.SdkStyle);
        await sut.DownloadForProjectAsync(project);

        context
            .For<IDownloadPackageReferencesViaDotnetCli>()
            .Verify(
                downloadViaDotnet =>
                    downloadViaDotnet.DownloadForProjectAsync(It.IsAny<Project>(), It.IsAny<IDirectoryInfo>()),
                Times.Once);

        context
            .For<IDownloadPackageReferencesViaNugetCli>()
            .Verify(downloadViaNuget =>
                    downloadViaNuget.DownloadForProjectAsync(It.IsAny<Project>(), It.IsAny<IDirectoryInfo>()),
                Times.Never);
    }

    [Fact]
    public async Task DownloadForProject_Uses_Nuget_Cli_For_Non_Sdk_Style_Project()
    {
        var context = ArrangeContext<DownloadPackageReferencesFacade>.Create();
        var sut = context.Build();

        context
            .For<IProvideTemporaryDirectories>()
            .Setup(provideTempDirectories => provideTempDirectories.GetDownloadDirectory())
            .Returns(Mock.Of<IDirectoryInfo>());

        var project = new Project("SomeProject", Mock.Of<IFileInfo>(), ProjectFormatStyle.NonSdkStyle);
        await sut.DownloadForProjectAsync(project);

        context
            .For<IDownloadPackageReferencesViaDotnetCli>()
            .Verify(
                downloadViaDotnet =>
                    downloadViaDotnet.DownloadForProjectAsync(It.IsAny<Project>(), It.IsAny<IDirectoryInfo>()),
                Times.Never);

        context
            .For<IDownloadPackageReferencesViaNugetCli>()
            .Verify(downloadViaNuget =>
                    downloadViaNuget.DownloadForProjectAsync(It.IsAny<Project>(), It.IsAny<IDirectoryInfo>()),
                Times.Once);
    }
}
