using ArrangeContext.Moq;
using FluentAssertions;
using Liz.Core.Logging.Contracts;
using Liz.Core.PackageReferences.Contracts.Models;
using Liz.Core.PackageReferences.Contracts.NuGetCli;
using Liz.Core.PackageReferences.NuGetCli;
using Liz.Core.Projects.Contracts.Models;
using Moq;
using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;
using Xunit;

namespace Liz.Core.Tests.PackageReferences.NuGetCli;

public class GetPackageReferencesViaPackagesConfigTests
{
    [Fact]
    public async Task GetFromProject_Fails_On_Invalid_Parameters()
    {
        var context = ArrangeContext<GetPackageReferencesViaPackagesConfig>.Create();
        var sut = context.Build();

        await Assert.ThrowsAsync<ArgumentNullException>(() => sut.GetFromProjectAsync(null!, true));
        await Assert.ThrowsAsync<ArgumentNullException>(() => sut.GetFromProjectAsync(null!, false));
    }

    [Fact]
    public async Task GetFromProject_Logs_Warning_When_Packages_Config_Not_Found()
    {
        const string projectFile = "./some/path/project.csproj";
        
        var mockFileSystem = new MockFileSystem();
        mockFileSystem.AddFile(projectFile, new MockFileData(string.Empty));

        var projectFileInfo = mockFileSystem.FileInfo.FromFileName(projectFile);

        var context = ArrangeContext<GetPackageReferencesViaPackagesConfig>.Create();
        context.Use<IFileSystem>(mockFileSystem);
        
        var sut = context.Build();

        var project = new Project("Something", projectFileInfo, ProjectFormatStyle.NonSdkStyle);
        var result = await sut.GetFromProjectAsync(project, true);

        result
            .Should()
            .BeEmpty();

        context
            .For<IParsePackagesConfigFile>()
            .Verify(parsePackagesConfig => parsePackagesConfig.GetPackageReferences(It.IsAny<IFileInfo>()),
                Times.Never);
        
        context
            .For<ILogger>()
            .Verify(logger => logger.Log(LogLevel.Warning, It.IsAny<string>(), null), Times.Once);
    }

    [Fact]
    public async Task GetFromProject()
    {
        const string projectFileDirectory = "./some/path";
        const string projectFile = $"{projectFileDirectory}/project.csproj";
        const string packagesConfigFile = $"{projectFileDirectory}/packages.config";
        
        var mockFileSystem = new MockFileSystem();
        mockFileSystem.AddFile(projectFile, new MockFileData(string.Empty));
        mockFileSystem.AddFile(packagesConfigFile, new MockFileData(string.Empty));

        var projectFileInfo = mockFileSystem.FileInfo.FromFileName(projectFile);

        var context = ArrangeContext<GetPackageReferencesViaPackagesConfig>.Create();
        context.Use<IFileSystem>(mockFileSystem);
        
        var packageReferences = new[]
        {
            new PackageReference("Something", "netSomething", "something"),
            new PackageReference("Autofac", "net472", "1.2.3")
        };

        context
            .For<IParsePackagesConfigFile>()
            .Setup(parsePackagesConfig => parsePackagesConfig.GetPackageReferences(It.IsAny<IFileInfo>()))
            .Returns(packageReferences);
        
        var sut = context.Build();

        var project = new Project("Something", projectFileInfo, ProjectFormatStyle.NonSdkStyle);
        var result = await sut.GetFromProjectAsync(project, true);

        result
            .Should()
            .BeEquivalentTo(packageReferences);
    }
    
    [Fact]
    public async Task GetFromProject_Logs_When_Include_Transitive_Is_Set_To_False()
    {
        const string projectFileDirectory = "./some/path";
        const string projectFile = $"{projectFileDirectory}/project.csproj";
        const string packagesConfigFile = $"{projectFileDirectory}/packages.config";
        
        var mockFileSystem = new MockFileSystem();
        mockFileSystem.AddFile(projectFile, new MockFileData(string.Empty));
        mockFileSystem.AddFile(packagesConfigFile, new MockFileData(string.Empty));

        var projectFileInfo = mockFileSystem.FileInfo.FromFileName(projectFile);

        var context = ArrangeContext<GetPackageReferencesViaPackagesConfig>.Create();
        context.Use<IFileSystem>(mockFileSystem);
        
        
        var sut = context.Build();

        var project = new Project("Something", projectFileInfo, ProjectFormatStyle.NonSdkStyle);
        _ = await sut.GetFromProjectAsync(project, false);

        context
            .For<ILogger>()
            .Verify(logger => logger.Log(LogLevel.Warning, It.IsAny<string>(), null), Times.Once);
    }
}
