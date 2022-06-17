using ArrangeContext.Moq;
using FluentAssertions;
using Liz.Core.Projects;
using Liz.Core.Projects.Contracts.Models;
using Moq;
using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;
using Xunit;

namespace Liz.Core.Tests.Projects;

public class GetProjectReferencesTests
{
    [Fact]
    public void Fails_On_Invalid_Parameters()
    {
        var context = ArrangeContext<GetProjectReferences>.Create();
        var sut = context.Build();

        Assert.Throws<ArgumentNullException>(() => sut.GetProjectReferenceNames(null!));
    }

    [Fact]
    public void Does_Nothing_When_Project_File_Does_Not_Exist()
    {
        var context = ArrangeContext<GetProjectReferences>.Create();
        var sut = context.Build();

        var projectFileMock = new Mock<IFileInfo>();
        projectFileMock
            .SetupGet(file => file.Exists)
            .Returns(false);

        var project = new Project("Something", projectFileMock.Object, ProjectFormatStyle.SdkStyle);

        var result = sut.GetProjectReferenceNames(project);

        result
            .Should()
            .BeEmpty();
    }

    [Fact]
    public void Gets_Project_Reference_Names()
    {
        var mockFileSystem = new MockFileSystem();
        
        var context = ArrangeContext<GetProjectReferences>.Create();
        context.Use<IFileSystem>(mockFileSystem);
        
        var sut = context.Build();
        
        mockFileSystem.AddFile("ProjectOne.csproj", new MockFileData(@"
<Project Sdk=""Microsoft.NET.Sdk"">
    <PropertyGroup>
        <AssemblyName>NameFromAssemblyName</AssemblyName>
        <PackageId>NameFromPackageId</PackageId>
    </PropertyGroup>

    <ItemGroup>
        <ProjectReference Include=""ProjectTwo.csproj"" />
    </ItemGroup>
</Project>"));
        
        mockFileSystem.AddFile("ProjectTwo.csproj", new MockFileData(@"
<Project Sdk=""Microsoft.NET.Sdk"">
    <PropertyGroup>
        <AssemblyName>NameFromAssemblyName</AssemblyName>
    </PropertyGroup>

    <ItemGroup>
        <ProjectReference Include=""ProjectThree.csproj"" />
    </ItemGroup>
</Project>"));
        
        mockFileSystem.AddFile("ProjectThree.csproj", new MockFileData(@"
<Project Sdk=""Microsoft.NET.Sdk"">
</Project>"));

        var projectOne = mockFileSystem.FileInfo.FromFileName("ProjectOne.csproj");
        var project = new Project("ProjectOne", projectOne, ProjectFormatStyle.SdkStyle);

        var result = sut.GetProjectReferenceNames(project);

        result
            .Should()
            .Contain("NameFromPackageId")
            .And
            .Contain("NameFromAssemblyName")
            .And
            .Contain("ProjectThree");
    }
}
