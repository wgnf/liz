using Liz.Core.Projects.Contracts;
using Liz.Core.Projects.Contracts.Models;
using System.IO.Abstractions;
using System.Xml.Linq;

namespace Liz.Core.Projects;

internal sealed class GetProjectReferences : IGetProjectReferences
{
    private readonly IFileSystem _fileSystem;

    public GetProjectReferences(IFileSystem fileSystem)
    {
        _fileSystem = fileSystem ?? throw new ArgumentNullException(nameof(fileSystem));
    }
    
    public IEnumerable<string> GetProjectReferenceNames(Project project)
    {
        if (project == null) throw new ArgumentNullException(nameof(project));

        var projectFile = project.File;
        var projectReferenceNames = GetProjectReferenceNames(projectFile);
        return projectReferenceNames.Distinct();
    }

    private IEnumerable<string> GetProjectReferenceNames(IFileInfo projectFile)
    {
        var projectReferenceNames = new List<string>();

        if (!projectFile.Exists) return projectReferenceNames;

        var projectFileContent = _fileSystem.File.ReadAllText(projectFile.FullName);

        var projectXml = XDocument.Parse(projectFileContent);
        var projectReferenceName = DetermineProjectReferenceName(projectFile, projectXml);

        projectReferenceNames.Add(projectReferenceName);

        GetProjectReferenceNamesFromProjectReferences(projectFile, projectXml, projectReferenceNames);

        return projectReferenceNames;
    }

    /*
     * NOTE:
     * the project-name (which is later used as the package-name i.e. when doing 'dotnet list package') is one of the
     * following (the higher up, the higher the priority)
     * i have not found any documentation on this, this is just something that i have come across
     *
     * - XML-Element "PackageId"
     * - XML-Element "AssemblyName"
     * - file-name
     */
    private string DetermineProjectReferenceName(IFileSystemInfo projectFile, XContainer projectXml)
    {
        var packageId = projectXml
            .Descendants("PackageId")
            .FirstOrDefault()?
            .Value;
        
        var assemblyName = projectXml
            .Descendants("AssemblyName")
            .FirstOrDefault()?
            .Value;
        
        var projectReferenceName = _fileSystem.Path.GetFileNameWithoutExtension(projectFile.FullName);

        if (!string.IsNullOrWhiteSpace(assemblyName))
            projectReferenceName = assemblyName;

        if (!string.IsNullOrWhiteSpace(packageId))
            projectReferenceName = packageId;
        return projectReferenceName;
    }

    private void GetProjectReferenceNamesFromProjectReferences(
        IFileInfo projectFile, 
        XContainer projectXml,
        List<string> projectReferenceNames)
    {
        var projectReferences = projectXml
            .Descendants("ProjectReference")
            .Select(element => element.Attribute("Include")?.Value)
            .Where(projectReference => !string.IsNullOrWhiteSpace(projectReference));

        foreach (var projectReference in projectReferences)
        {
            // the project-reference is a relative path of the current project-file
            var projectReferencePath = _fileSystem.Path.Combine(projectFile.Directory.FullName, projectReference);
            var projectReferenceInfo = _fileSystem.FileInfo.FromFileName(projectReferencePath);

            var subProjectReferenceNames = GetProjectReferenceNames(projectReferenceInfo);
            projectReferenceNames.AddRange(subProjectReferenceNames);
        }
    }
}
