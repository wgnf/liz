﻿using ArrangeContext.Moq;
using Liz.Core.CliTool.Contracts;
using Liz.Core.PackageReferences.NuGetCli;
using Liz.Core.Projects.Contracts.Models;
using Moq;
using System.IO.Abstractions;
using Xunit;

namespace Liz.Core.Tests.PackageReferences.NuGetCli;

public class DownloadPackageReferencesViaNugetCliTests
{
    [Fact]
    public async Task DownloadForProject_Fails_On_Invalid_Parameters()
    {
        var context = ArrangeContext<DownloadPackageReferencesViaNugetCli>.Create();
        var sut = context.Build();

        await Assert.ThrowsAsync<ArgumentNullException>(() =>
            sut.DownloadForProjectAsync(null!, Mock.Of<IDirectoryInfo>()));

        var project = new Project("SomeProject", Mock.Of<IFileInfo>(), ProjectFormatStyle.Unknown);
        await Assert.ThrowsAsync<ArgumentNullException>(() =>
            sut.DownloadForProjectAsync(project, null!));
    }

    [Fact]
    public async Task DownloadForProject()
    {
        var context = ArrangeContext<DownloadPackageReferencesViaNugetCli>.Create();
        var sut = context.Build();

        var targetDirectory = new Mock<IDirectoryInfo>();
        targetDirectory
            .Setup(directory => directory.CreateSubdirectory(It.IsAny<string>()))
            .Returns(Mock.Of<IDirectoryInfo>());

        var project = new Project("SomeProject", Mock.Of<IFileInfo>(), ProjectFormatStyle.Unknown);
        await sut.DownloadForProjectAsync(project, targetDirectory.Object);

        context
            .For<ICliToolExecutor>()
            .Verify(executor => executor.ExecuteAsync(
                    It.Is<string>(file => file == "nuget"), 
                    It.IsAny<string>()), 
                Times.Once);
    }
}