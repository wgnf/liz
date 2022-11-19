using ArrangeContext.Moq;
using FluentAssertions;
using Liz.Core.CliTool.Contracts;
using Liz.Core.PackageReferences.Contracts;
using Liz.Core.PackageReferences.Contracts.Models;
using Liz.Core.PackageReferences.DotnetCli;
using Liz.Core.Projects.Contracts;
using Liz.Core.Projects.Contracts.Models;
using Moq;
using System.IO.Abstractions;
using Xunit;

namespace Liz.Core.Tests.PackageReferences.DotnetCli;

public class GetPackageReferencesViaDotnetCliTests
{
    [Fact]
    public async Task GetFromProject_Should_Throw_On_Invalid_Parameters()
    {
        var context = new ArrangeContext<GetPackageReferencesViaDotnetCli>();
        var sut = context.Build();

        await Assert.ThrowsAsync<ArgumentNullException>(() => sut.GetFromProjectAsync(null!, false));
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task GetFromProject(bool includeTransitive)
    {
        var context = new ArrangeContext<GetPackageReferencesViaDotnetCli>();
        var sut = context.Build();

        var expectedPackageReferences = new List<PackageReference> { new("something", "something", "something") };

        context
            .For<IParseDotnetListPackageResult>()
            .Setup(parser => parser.Parse(It.IsAny<string>()))
            .Returns(expectedPackageReferences);

        var project = new Project("Something", Mock.Of<IFileInfo>(), ProjectFormatStyle.SdkStyle);
        var packageReferences = await sut.GetFromProjectAsync(project, includeTransitive);

        packageReferences
            .Should()
            .BeEquivalentTo(expectedPackageReferences);

        context
            .For<ICliToolExecutor>()
            .Verify(executor => executor.ExecuteAsync(
                    It.Is<string>(fileName => fileName == "dotnet"),
                    It.Is<string>(arguments => arguments.Contains("restore"))),
                Times.Once);

        context
            .For<ICliToolExecutor>()
            .Verify(executor => executor.ExecuteWithResultAsync(
                    It.Is<string>(fileName => fileName == "dotnet"),
                    It.Is<string>(arguments =>
                        arguments.Contains("list") &&
                        arguments.Contains("package") &&
                        (!includeTransitive || arguments.Contains("include-transitive")))),
                Times.Once);
    }

    [Fact]
    public async Task GetFromProject_Removes_Project_References_From_Package_References()
    {
        var context = new ArrangeContext<GetPackageReferencesViaDotnetCli>();
        var sut = context.Build();
        
        var packageReferences = new List<PackageReference>
        {
            new("something", "something", "something"),
            new("liz.something", "something", "something"),
            new("liz.core.something", "something","something")
        };

        context
            .For<IParseDotnetListPackageResult>()
            .Setup(parser => parser.Parse(It.IsAny<string>()))
            .Returns(packageReferences);

        context
            .For<IGetProjectReferences>()
            .Setup(getProjectReferences => getProjectReferences.Get(It.IsAny<Project>()))
            .Returns(new[]
            {
                new ProjectReference("liz.something", "liz.something.csproj"),
                new ProjectReference("liz.core.something", "liz.core.something.csproj")
            });
        
        var project = new Project("Something", Mock.Of<IFileInfo>(), ProjectFormatStyle.SdkStyle);
        var result = await sut.GetFromProjectAsync(project, true);

        result
            .Should()
            .OnlyContain(package => package.Name == "something");
    }
}
