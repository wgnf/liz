using Liz.Core.CliTool.Contracts;
using Liz.Core.PackageReferences.DotnetCli;
using Liz.Core.Projects.Contracts.Models;
using Moq;
using System.IO.Abstractions;
using Xunit;
using ArrangeContext.Moq;

namespace Liz.Core.Tests.PackageReferences.DotnetCli;

public class DownloadPackageReferencesViaDotnetCliTests
{
    [Fact]
    public async Task Download_Fails_On_Invalid_Parameters()
    {
        var context = ArrangeContext<DownloadPackageReferencesViaDotnetCli>.Create();
        var sut = context.Build();

        await Assert.ThrowsAsync<ArgumentNullException>(() =>
            sut.DownloadAsync(null!, Mock.Of<IDirectoryInfo>()));

        await Assert.ThrowsAsync<ArgumentNullException>(() =>
            sut.DownloadAsync(Mock.Of<IFileInfo>(), null!));
    }

    [Fact]
    public async Task Download()
    {
        var context = ArrangeContext<DownloadPackageReferencesViaDotnetCli>.Create();
        var sut = context.Build();

        var targetDirectory = new Mock<IDirectoryInfo>();
        targetDirectory
            .Setup(directory => directory.CreateSubdirectory(It.IsAny<string>()))
            .Returns(Mock.Of<IDirectoryInfo>());

        var project = new Project("SomeProject", Mock.Of<IFileInfo>(), ProjectFormatStyle.Unknown);
        await sut.DownloadAsync(Mock.Of<IFileInfo>(), targetDirectory.Object);

        context
            .For<ICliToolExecutor>()
            .Verify(executor => executor.ExecuteAsync(
                    It.Is<string>(file => file == "dotnet"), 
                    It.IsAny<string>()), 
                Times.Once);
    }
}
