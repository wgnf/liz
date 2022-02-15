using Liz.Core.Logging;
using Liz.Core.Logging.Contracts;
using Liz.Core.PackageReferences.Contracts.Models;
using Liz.Core.PackageReferences.Contracts.NuGetCli;
using Liz.Core.Projects.Contracts.Models;
using System.IO.Abstractions;

namespace Liz.Core.PackageReferences.NuGetCli;

internal sealed class GetPackageReferencesViaPackagesConfig : IGetPackageReferencesViaPackagesConfig
{
    private readonly ILogger _logger;
    private readonly IFileSystem _fileSystem;
    private readonly IParsePackagesConfigFile _parsePackagesConfigFile;

    public GetPackageReferencesViaPackagesConfig(
        ILogger logger, 
        IFileSystem fileSystem,
        IParsePackagesConfigFile parsePackagesConfigFile)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _fileSystem = fileSystem ?? throw new ArgumentNullException(nameof(fileSystem));
        _parsePackagesConfigFile = parsePackagesConfigFile ?? throw new ArgumentNullException(nameof(parsePackagesConfigFile));
    }
    
    public Task<IEnumerable<PackageReference>> GetFromProjectAsync(Project project, bool includeTransitive)
    {
        if (project == null) throw new ArgumentNullException(nameof(project));

        if (!includeTransitive)
            _logger.LogWarning("The option 'include-transitive' is disabled, but you're analyzing a " +
                               "non-SDK-Style-Project which includes transitive references by default!");

        if (!TryGetPackagesConfigFile(project, out var packagesConfigFile) || packagesConfigFile == null)
        {
            _logger.LogWarning($"Unable to find packages.config file next to '{project.File}'! " +
                               $"Cannot determine package-references for project '{project.Name}'");
            return Task.FromResult(Enumerable.Empty<PackageReference>());
        }

        var packageReferences = _parsePackagesConfigFile.GetPackageReferences(packagesConfigFile);
        return Task.FromResult(packageReferences);
    }

    private bool TryGetPackagesConfigFile(Project project, out IFileInfo? packagesConfigFile)
    {
        packagesConfigFile = null;
        
        var directoryOfProject = project.File.Directory;
        var potentialPackagesConfig = _fileSystem.Path.Combine(directoryOfProject.FullName, "packages.config");
        if (string.IsNullOrWhiteSpace(potentialPackagesConfig))
            return false;

        packagesConfigFile = _fileSystem.FileInfo.FromFileName(potentialPackagesConfig);
        return packagesConfigFile.Exists;
    }
}
