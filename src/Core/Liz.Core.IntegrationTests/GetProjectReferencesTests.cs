using FluentAssertions;
using Liz.Core.Projects;
using Liz.Core.Projects.Contracts.Models;
using System.IO.Abstractions;
using Xunit;

namespace Liz.Core.IntegrationTests;

public class GetProjectReferencesTests
{
    [Fact]
    public void Gets_Project_References_Of_Liz_Tool_Project_Correctly()
    {
        const string lizToolProjectPath = "../../../../../Tool/Liz.Tool/Liz.Tool.csproj";
        
        var fileSystem = new FileSystem();
        var lizToolProjectFile = fileSystem.FileInfo.FromFileName(lizToolProjectPath);
        lizToolProjectFile
            .Exists
            .Should()
            .BeTrue();
        
        var getProjectReferences = new GetProjectReferences(fileSystem);

        var lizToolProject = new Project("Liz.Tool", lizToolProjectFile, ProjectFormatStyle.SdkStyle);
        var projectReferences = getProjectReferences.Get(lizToolProject);

        projectReferences
            .Should()
            .Contain(reference => reference.Name == "Liz.Core")
            .And
            .Contain(reference => reference.Name == "Liz.Tool");
    }
}
