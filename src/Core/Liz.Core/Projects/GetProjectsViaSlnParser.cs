using Liz.Core.Projects.Contracts;
using Liz.Core.Projects.Contracts.Models;
using Liz.Core.Settings;
using SlnParser.Contracts;
using System.IO.Abstractions;
using System.Xml.Linq;

namespace Liz.Core.Projects;

internal sealed class GetProjectsViaSlnParser : IGetProjects
{
    private readonly IFileSystem _fileSystem;
    private readonly IGetProjectReferences _getProjectReferences;
    private readonly ExtractLicensesSettingsBase _settings;
    private readonly ISolutionParser _solutionParser;

    public GetProjectsViaSlnParser(
        ExtractLicensesSettingsBase settings,
        ISolutionParser solutionParser, 
        IFileSystem fileSystem,
        IGetProjectReferences getProjectReferences)
    {
        _settings = settings ?? throw new ArgumentNullException(nameof(settings));
        _solutionParser = solutionParser ?? throw new ArgumentNullException(nameof(solutionParser));
        _fileSystem = fileSystem ?? throw new ArgumentNullException(nameof(fileSystem));
        _getProjectReferences = getProjectReferences ?? throw new ArgumentNullException(nameof(getProjectReferences));
    }

    public IEnumerable<Project> GetFromFile(string targetFile)
    {
        if (string.IsNullOrWhiteSpace(targetFile))
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(targetFile));

        var fileInfo = _fileSystem.FileInfo.FromFileName(targetFile);
        
        ValidateProvidedTargetFile(fileInfo);
        var projects = GetProjects(fileInfo);
        return projects;
    }

    private static void ValidateProvidedTargetFile(IFileSystemInfo targetFile)
    {
        if (!targetFile.Exists)
            throw new FileNotFoundException($"The provided target-file '{targetFile.FullName}' could not be found");

        if (targetFile.Extension is not (".sln" or ".csproj" or ".fsproj"))
            throw new ArgumentException(
                $"The provided target-file '{targetFile.FullName}' is not of the right type " +
                "(only 'sln', 'csproj' and 'fsproj' is supported");
    }

    private IEnumerable<Project> GetProjects(IFileInfo targetFile)
    {
        return targetFile.Extension switch
        {
            ".csproj" or ".fsproj" => GetProjectFromProjectFile(targetFile),
            ".sln" => GetProjectFromSolutionFile(targetFile),
            _ => throw new ArgumentOutOfRangeException(nameof(targetFile),
                $"Target-file-extension was expected to be 'csproj', 'fsproj' or 'sln' but found '{targetFile.Extension}'")
        };
    }

    private IEnumerable<Project> GetProjectFromProjectFile(IFileInfo targetFile)
    {
        // NOTE: we just assume that the Project-Name == File-Name here (it's not all too important, because it'll only be used for logs) 
        var projectName = _fileSystem.Path.GetFileNameWithoutExtension(targetFile.FullName);
        var project = new Project(projectName, targetFile, DetermineProjectFormatStyle(targetFile.FullName));

        if (!_settings.IncludeTransitiveDependencies || project.FormatStyle != ProjectFormatStyle.SdkStyle)
            return new[] { project };
        
        /*
         * NOTE:
         * if 'include transitive' is activated and 'project' is SDK-Style we have to check if there are any project-references to
         * a Non-SDK-Style project, because package-references of these projects are not correctly included otherwise
         *
         * c.f.: https://github.com/wgnf/liz/issues/116
         */
        var projectReferencesNonSdkStyle = DetermineProjectReferencesNonSdkStyle(project);
        var projects = new[] { project }.Concat(projectReferencesNonSdkStyle);

        return projects;
    }

    private IEnumerable<Project> GetProjectFromSolutionFile(IFileSystemInfo fileInfo)
    {
        var solution = _solutionParser.Parse(fileInfo.FullName);
        var projects = solution
            .AllProjects
            .OfType<SolutionProject>()
            .Where(project => project.File.Extension is ".csproj" or ".fsproj")
            .Where(project => _fileSystem.File.Exists(project.File.FullName))
            .Select(project => 
                new Project(
                    project.Name, 
                    _fileSystem.FileInfo.FromFileName(project.File.FullName), 
                    DetermineProjectFormatStyle(project.File.FullName)))
            .ToList();

        return projects;
    }

    private ProjectFormatStyle DetermineProjectFormatStyle(string projectFile)
    {
        try
        {
            return IsSdkStyle(projectFile)
                ? ProjectFormatStyle.SdkStyle
                : ProjectFormatStyle.NonSdkStyle;
        }
        catch (Exception)
        {
            return ProjectFormatStyle.Unknown;
        }
    }
    
    private bool IsSdkStyle(string projectFile)
    {
        using var fileStream = _fileSystem.FileStream.Create(projectFile, FileMode.Open, FileAccess.Read);
        var xmlDocument = XDocument.Load(fileStream);

        var projectRoot = xmlDocument
            .Elements("Project")
            .FirstOrDefault();

        var hasSdkAttribute = projectRoot?.Attribute("Sdk") != null;
        var hasSdkElement = xmlDocument.Descendants("Sdk").Any();

        return hasSdkAttribute || hasSdkElement;
    }

    private IEnumerable<Project> DetermineProjectReferencesNonSdkStyle(Project startingProject)
    {
        var projectReferences = _getProjectReferences.Get(startingProject);
        var projectReferencesWithStyle = projectReferences
            .Select(projectReference => new
            {
                Reference = projectReference, 
                Style = DetermineProjectFormatStyle(projectReference.FileName)
            });

        return projectReferencesWithStyle
            .Where(projectReferenceWithStyle => projectReferenceWithStyle.Style == ProjectFormatStyle.NonSdkStyle)
            .Select(projectReferenceWithStyle =>
            {
                var projectName = _fileSystem.Path.GetFileNameWithoutExtension(projectReferenceWithStyle.Reference.FileName);
                var fileInfo = _fileSystem.FileInfo.FromFileName(projectReferenceWithStyle.Reference.FileName);

                return new Project(projectName, fileInfo, projectReferenceWithStyle.Style);
            });
    }
}
