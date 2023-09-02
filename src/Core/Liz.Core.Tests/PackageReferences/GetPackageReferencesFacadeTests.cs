using ArrangeContext.Moq;
using Liz.Core.PackageReferences;
using Liz.Core.PackageReferences.Contracts.DotnetCli;
using Liz.Core.PackageReferences.Contracts.NuGetCli;
using Liz.Core.Projects.Contracts.Models;
using Moq;
using System.IO.Abstractions;
using FluentAssertions;
using Liz.Core.PackageReferences.Contracts.Models;
using Liz.Core.Settings;
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
    
    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task GetFromProject_And_Get_From_Packages_Config_On_Non_Sdk_Style_Format(bool includeTransitive)
    {
        var context = new ArrangeContext<GetPackageReferencesFacade>();
        var sut = context.Build();

        var projectFileMock = new Mock<IFileInfo>();
        projectFileMock
            .SetupGet(file => file.Exists)
            .Returns(true);

        var project = new Project("Something", projectFileMock.Object, ProjectFormatStyle.NonSdkStyle);

        await sut.GetFromProjectAsync(project, includeTransitive);
        
        context
            .For<IGetPackageReferencesViaPackagesConfig>()
            .Verify(getPackage => getPackage.GetFromProjectAsync(project, includeTransitive),
                Times.Once);
    }
    
    [Fact]
    public async Task GetFromProject_And_Exclude_Packages_According_To_Globs()
    {
        var settings = Mock.Of<ExtractLicensesSettingsBase>();
        settings.PackageExclusionGlobs.Add("Test*");
        
        var context = new ArrangeContext<GetPackageReferencesFacade>();
        context.Use(settings);
        
        var sut = context.Build();

        var packages = new[]
        {
            new PackageReference("Test", "net6.0", "1.2.3"), 
            new PackageReference("Test.Core", "net6.0", "1.2.3"),
            new PackageReference("Test.Utils.Core", "net6.0", "1.2.3"),
            new PackageReference("Testinson", "net6.0", "1.2.3"), 
            new PackageReference("Martin", "net6.0", "1.2.3")
        };

        context
            .For<IGetPackageReferencesViaDotnetCli>()
            .Setup(getPackageReferences => getPackageReferences.GetFromProjectAsync(It.IsAny<Project>(), It.IsAny<bool>()))
            .ReturnsAsync(packages);

        var projectFileMock = new Mock<IFileInfo>();
        projectFileMock
            .SetupGet(file => file.Exists)
            .Returns(true);

        var project = new Project("Something", projectFileMock.Object, ProjectFormatStyle.SdkStyle);

        var result = (await sut.GetFromProjectAsync(project, true)).ToList();

        result
            .Should()
            .HaveCount(1);

        result
            .Should()
            .OnlyContain(package => package.Name == "Martin");
    }
}
