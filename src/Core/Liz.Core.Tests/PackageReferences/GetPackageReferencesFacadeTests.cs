using ArrangeContext.Moq;
using Liz.Core.Logging;
using Liz.Core.PackageReferences;
using Liz.Core.PackageReferences.DotnetCli;
using Liz.Core.Projects;
using Moq;
using System;
using System.IO;
using System.IO.Abstractions;
using System.Threading.Tasks;
using Xunit;

namespace Liz.Core.Tests.PackageReferences;

public class GetPackageReferencesFacadeTests
{
    [Fact]
    public async Task GetFromProject_And_Throw_On_Invalid_Parameters()
    {
        var context = new ArrangeContext<GetPackageReferencesFacade>();
        var sut = context.Build();

        await Assert.ThrowsAsync<ArgumentNullException>(() => sut.GetFromProjectAsync(null!, false));
    }

    [Fact]
    public async Task GetFromProject_And_Throw_When_File_Not_Found()
    {
        var context = new ArrangeContext<GetPackageReferencesFacade>();
        var sut = context.Build();

        var projectFileMock = new Mock<IFileInfo>();
        projectFileMock
            .SetupGet(file => file.Exists)
            .Returns(false);

        var project = new Project("Something", projectFileMock.Object, ProjectFormatStyle.Unknown);

        await Assert.ThrowsAsync<FileNotFoundException>(() => sut.GetFromProjectAsync(project, false));
    }

    [Fact]
    public async Task GetFromProject_And_Throw_On_Unknown_Format()
    {
        var context = new ArrangeContext<GetPackageReferencesFacade>();
        var sut = context.Build();

        var projectFileMock = new Mock<IFileInfo>();
        projectFileMock
            .SetupGet(file => file.Exists)
            .Returns(true);

        var project = new Project("Something", projectFileMock.Object, ProjectFormatStyle.Unknown);

        await Assert.ThrowsAsync<ArgumentException>(() => sut.GetFromProjectAsync(project, false));
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task GetFromProject_And_Get_From_Dotnet_Cli_On_Sdk_Style_Format(bool includeTransitive)
    {
        var context = new ArrangeContext<GetPackageReferencesFacade>();
        var sut = context.Build();

        var projectFileMock = new Mock<IFileInfo>();
        projectFileMock
            .SetupGet(file => file.Exists)
            .Returns(true);

        var project = new Project("Something", projectFileMock.Object, ProjectFormatStyle.SdkStyle);

        await sut.GetFromProjectAsync(project, includeTransitive);

        context
            .For<IGetPackageReferencesViaDotnetCli>()
            .Verify(getPackage => getPackage.GetFromProjectAsync(project, includeTransitive),
                Times.Once);
    }
    
    [Fact]
    public async Task GetFromProject_And_Log_Warning_On_NonSdk_Style_Format()
    {
        var context = new ArrangeContext<GetPackageReferencesFacade>();
        var sut = context.Build();

        var projectFileMock = new Mock<IFileInfo>();
        projectFileMock
            .SetupGet(file => file.Exists)
            .Returns(true);

        var project = new Project("Something", projectFileMock.Object, ProjectFormatStyle.NonSdkStyle);

        await sut.GetFromProjectAsync(project, false);
        
        context
            .For<ILogger>()
            .Verify(logger => logger.Log(LogLevel.Warning, It.IsAny<string>(), null),
                Times.Once);
    }
}
