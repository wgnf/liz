using JetBrains.Annotations;
using Liz.Core.License;
using Liz.Core.Logging;
using Liz.Core.PackageReferences;
using Liz.Core.Projects;
using Liz.Core.Settings;
using Liz.Core.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Liz.Core.Extract;

internal sealed class ExtractLicenses : IExtractLicenses
{
    private readonly IGetPackageReferences _getPackageReferences;
    private readonly IGetLicenseInformation _getLicenseInformation;
    private readonly IProvideTemporaryDirectory _provideTemporaryDirectory;
    private readonly IGetProjects _getProjects;
    private readonly ILogger _logger;
    private readonly ExtractLicensesSettings _settings;

    public ExtractLicenses(
        [NotNull] ExtractLicensesSettings settings,
        [NotNull] ILogger logger,
        [NotNull] IGetProjects getProjects,
        [NotNull] IGetPackageReferences getPackageReferences,
        [NotNull] IGetLicenseInformation getLicenseInformation,
        [NotNull] IProvideTemporaryDirectory provideTemporaryDirectory)
    {
        _settings = settings ?? throw new ArgumentNullException(nameof(settings));
        _getProjects = getProjects ?? throw new ArgumentNullException(nameof(getProjects));
        _getPackageReferences = getPackageReferences ?? throw new ArgumentNullException(nameof(getPackageReferences));
        _getLicenseInformation = getLicenseInformation ?? throw new ArgumentNullException(nameof(getLicenseInformation));
        _provideTemporaryDirectory = provideTemporaryDirectory 
                                     ?? throw new ArgumentNullException(nameof(provideTemporaryDirectory));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task ExtractAsync()
    {
        try
        {
            var projects = GetProjects(_settings.TargetFile);
            var packageReferences = await GetPackageReferencesAsync(projects);
            var licenseInformation = await GetLicenseInformationAsync(packageReferences);
        }
        catch (Exception ex)
        {
            _logger.LogCritical($"Error occured while extracting licenses for '{_settings.TargetFile}'", ex);
        }
        finally
        {
            CleanUpTemporaryDirectory();
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
            throw new GetProjectsFailedException(targetFile, ex);
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
            throw new GetPackageReferencesFailedException(project, ex);
        }
    }

    private async Task<IEnumerable<LicenseInformation>> GetLicenseInformationAsync(
        IEnumerable<PackageReference> packageReferences)
    {
        var licenseInformation = new List<LicenseInformation>();
        
        _logger.LogDebug("Trying to get license information for package(s)...");

        foreach (var packageReference in packageReferences)
        {
            var licenseInformationFromPackageReference =
                await GetLicenseInformationForPackageReferenceAsync(packageReference);
            licenseInformation.Add(licenseInformationFromPackageReference);
        }

        return licenseInformation;
    }

    private async Task<LicenseInformation> GetLicenseInformationForPackageReferenceAsync(PackageReference packageReference)
    {
        try
        {
            _logger.LogDebug($"Trying to get license information for {packageReference}...");

            var licenseInformation = await _getLicenseInformation.GetFromPackageReferenceAsync(packageReference);
            _logger.LogDebug($"Found following license-type: '{licenseInformation.LicenseType}'");

            return licenseInformation;
        }
        catch (Exception ex)
        {
            throw new GetLicenseInformationFailedException(packageReference, ex);
        }
    }

    private void CleanUpTemporaryDirectory()
    {
        var temporaryDirectory = _provideTemporaryDirectory.Get();
        temporaryDirectory.Delete(true);
    }
}
