using DotnetNugetLicenses.Core.Logging;
using DotnetNugetLicenses.Core.PackageReferences;
using DotnetNugetLicenses.Core.Projects;
using DotnetNugetLicenses.Core.Settings;
using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DotnetNugetLicenses.Core.Extract;

internal sealed class ExtractLicenses : IExtractLicenses
{
    private readonly IGetPackageReferences _getPackageReferences;
    private readonly IGetProjects _getProjects;
    private readonly ILogger _logger;
    private readonly ExtractLicensesSettings _settings;

    public ExtractLicenses(
        [NotNull] ExtractLicensesSettings settings,
        [NotNull] ILogger logger,
        [NotNull] IGetProjects getProjects,
        [NotNull] IGetPackageReferences getPackageReferences)
    {
        _settings = settings ?? throw new ArgumentNullException(nameof(settings));
        _getProjects = getProjects ?? throw new ArgumentNullException(nameof(getProjects));
        _getPackageReferences = getPackageReferences ?? throw new ArgumentNullException(nameof(getPackageReferences));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task ExtractAsync()
    {
        try
        {
            var projects = GetProjects(_settings.TargetFile);
            var packageReferences = await GetPackageReferencesAsync(projects);
        }
        catch (Exception ex)
        {
            _logger.LogCritical($"Error occured while extracting licenses for '{_settings.TargetFile}'", ex);
        }
    }

    private IEnumerable<Project> GetProjects(string targetFile)
    {
        try
        {
            _logger.LogDebug($"Trying to get projects from {targetFile}...");

            var projects = _getProjects.GetFromFile(targetFile).ToList();

            var foundProjectsLogString = string.Join("\n", projects.Select(project => $"\t- {project.File.FullName}"));
            _logger.LogDebug($"Found following projects for '{targetFile}':\n{foundProjectsLogString}");

            return projects;
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Could not determine projects for file '{targetFile}'", ex);
        }
    }

    private async Task<IEnumerable<PackageReference>> GetPackageReferencesAsync(IEnumerable<Project> projects)
    {
        var packageReferences = new List<PackageReference>();

        _logger.LogDebug("Trying to get package-references for project(s)...");
        foreach (var project in projects)
        {
            var referencesFromProject = await GetPackageReferencesForProjectAsync(project);
            packageReferences.AddRange(referencesFromProject);
        }

        return packageReferences;
    }

    private async Task<IEnumerable<PackageReference>> GetPackageReferencesForProjectAsync(Project project)
    {
        try
        {
            _logger.LogDebug($"Trying to get package-references for project '{project.Name} ({project.File})'...");

            var packageReferences = (await _getPackageReferences
                .GetFromProjectAsync(project, _settings.IncludeTransitiveDependencies)).ToList();

            var foundReferencesLogString = string.Join("\n",
                packageReferences.Select(reference =>
                    $"\t- [{reference.TargetFramework}] {reference.Name} ({reference.Version})"));
            _logger.LogDebug(
                $"Found following package-references for '{project.Name} ({project.File})':\n{foundReferencesLogString}");

            return packageReferences;
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException(
                $"Could not determine package-references for project '{project.Name} ({project.File})'", ex);
        }
    }
}
