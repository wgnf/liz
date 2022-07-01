using Liz.Core.Projects.Contracts;
using Liz.Core.Projects.Contracts.Models;
using System.IO.Abstractions;
using System.Xml;
using System.Xml.Linq;

namespace Liz.Core.Projects;

internal sealed class GetProjectReferences : IGetProjectReferences
{
    private readonly IFileSystem _fileSystem;

    private readonly List<ProjectReference> _projectReferencesCache = new();

    public GetProjectReferences(IFileSystem fileSystem)
    {
        _fileSystem = fileSystem ?? throw new ArgumentNullException(nameof(fileSystem));
    }
    
    public IEnumerable<ProjectReference> Get(Project project)
    {
        if (project == null) throw new ArgumentNullException(nameof(project));

        var projectFile = project.File;
        if (!projectFile.Exists) return Enumerable.Empty<ProjectReference>();
        
        var projectReference = DoGetProjectReference(projectFile);
        var projectReferences = FlattenProjectReferences(projectReference).Distinct();

        return projectReferences;
    }

    private ProjectReference DoGetProjectReference(IFileInfo projectFile)
    {
        var projectFileContent = _fileSystem.File.ReadAllText(projectFile.FullName);
        var projectReferenceName = DetermineProjectReferenceName(projectFileContent, projectFile);

        // if something is in the (local class-)cache, we don't have to run the whole thing again
        var cachedCandidate = _projectReferencesCache
            .FirstOrDefault(reference => reference.Name == projectReferenceName);
        if (cachedCandidate != null) return cachedCandidate;

        var currentProjectReference = new ProjectReference(projectReferenceName);
        var subProjectReferenceFiles = DetermineSubProjectReferenceFiles(projectFileContent, projectFile);

        foreach (var subProjectReferenceFile in subProjectReferenceFiles)
        {
            var subProjectReferences = DoGetProjectReference(subProjectReferenceFile);
            currentProjectReference.AddReference(subProjectReferences);
        }
        
        _projectReferencesCache.Add(currentProjectReference);
        return currentProjectReference;
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
    private string DetermineProjectReferenceName(string projectFileContent, IFileSystemInfo projectFile)
    {
        // this somehow only works reliably using XDocument?!
        var projectXml = XDocument.Parse(projectFileContent);
        
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

    private IEnumerable<IFileInfo> DetermineSubProjectReferenceFiles(string projectFileContent, IFileInfo projectFile)
    {
        // this somehow only works reliably using XmlDocument?!
        var projectXml = new XmlDocument();
        projectXml.LoadXml(projectFileContent);
        
        var projectReferencePaths = new List<string>();

        var projectReferenceNodes = projectXml.GetElementsByTagName("ProjectReference");
        // ReSharper disable once LoopCanBeConvertedToQuery
        foreach (XmlNode? projectReferenceNode in projectReferenceNodes)
        {
            if (!TryGetProjectReferencePath(projectReferenceNode, out var projectReferencePath)) continue;
            
            projectReferencePaths.Add(projectReferencePath);
        }
        
        // the project-reference is a relative path of the current project-file
        var projectReferenceFiles = projectReferencePaths
            .Select(path => _fileSystem.Path.Combine(projectFile.Directory.FullName, path))
            .Select(combinedPath => _fileSystem.FileInfo.FromFileName(combinedPath))
            .Where(file => file.Exists);

        return projectReferenceFiles;
    }
    
    private static bool TryGetProjectReferencePath(XmlNode? projectReferenceNode, out string projectReferencePath)
    {
        projectReferencePath = string.Empty;   
        
        var projectReferenceNodeAttributes = projectReferenceNode?.Attributes;
        if (projectReferenceNodeAttributes == null) return false;

        // ReSharper disable once LoopCanBeConvertedToQuery
        foreach (XmlAttribute? projectReferenceAttribute in projectReferenceNodeAttributes)
        {
            if (projectReferenceAttribute?.Name != "Include") continue;
                
            projectReferencePath = projectReferenceAttribute.Value;
            
            // first thing we got is the only thing we need
            return true;
        }

        return false;
    }
    
    private static IEnumerable<ProjectReference> FlattenProjectReferences(ProjectReference projectReference)
    {
        var projectReferences = new List<ProjectReference>(new[] { projectReference });

        foreach (var reference in projectReference.ProjectReferences)
        {
            var subReferences = FlattenProjectReferences(reference);
            projectReferences.AddRange(subReferences);
        }

        return projectReferences;
    }
}
