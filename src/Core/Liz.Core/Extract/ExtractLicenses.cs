using Liz.Core.Extract.Contracts;
using Liz.Core.License.Contracts;
using Liz.Core.License.Contracts.Exceptions;
using Liz.Core.Logging;
using Liz.Core.Logging.Contracts;
using Liz.Core.PackageReferences.Contracts;
using Liz.Core.PackageReferences.Contracts.Exceptions;
using Liz.Core.PackageReferences.Contracts.Models;
using Liz.Core.Projects.Contracts;
using Liz.Core.Projects.Contracts.Exceptions;
using Liz.Core.Projects.Contracts.Models;
using Liz.Core.Settings;
using Liz.Core.Utils.Contracts;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;

namespace Liz.Core.Extract;

internal sealed class ExtractLicenses : IExtractLicenses
{
    private readonly IGetPackageReferences _getPackageReferences;
    private readonly IEnrichPackageReferenceWithLicenseInformation _enrichPackageReferenceWithLicenseInformation;
    private readonly IProvideTemporaryDirectories _provideTemporaryDirectories;
    private readonly IDownloadPackageReferences _downloadPackageReferences;
    private readonly IPackageReferencePrinter _packageReferencePrinter;
    private readonly IGetProjects _getProjects;
    private readonly ILogger _logger;
    private readonly ExtractLicensesSettings _settings;

    public ExtractLicenses(
        [NotNull] ExtractLicensesSettings settings,
        [NotNull] ILogger logger,
        [NotNull] IGetProjects getProjects,
        [NotNull] IGetPackageReferences getPackageReferences,
        [NotNull] IEnrichPackageReferenceWithLicenseInformation enrichPackageReferenceWithLicenseInformation,
        [NotNull] IProvideTemporaryDirectories provideTemporaryDirectories,
        [NotNull] IDownloadPackageReferences downloadPackageReferences,
        [NotNull] IPackageReferencePrinter packageReferencePrinter)
    {
        _settings = settings ?? throw new ArgumentNullException(nameof(settings));
        _getProjects = getProjects ?? throw new ArgumentNullException(nameof(getProjects));
        _getPackageReferences = getPackageReferences ?? throw new ArgumentNullException(nameof(getPackageReferences));
        _enrichPackageReferenceWithLicenseInformation = enrichPackageReferenceWithLicenseInformation ?? 
                                                        throw new ArgumentNullException(nameof(enrichPackageReferenceWithLicenseInformation));
        _provideTemporaryDirectories = provideTemporaryDirectories 
                                     ?? throw new ArgumentNullException(nameof(provideTemporaryDirectories));
        _downloadPackageReferences = downloadPackageReferences ?? throw new ArgumentNullException(nameof(downloadPackageReferences));
        _packageReferencePrinter = packageReferencePrinter ?? throw new ArgumentNullException(nameof(packageReferencePrinter));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<IEnumerable<PackageReference>> ExtractAsync()
    {
        try
        {
            var projects = GetProjects(_settings.TargetFile).ToList();

            await DownloadPackageReferencesFor(projects);
            
            var packageReferences = (await GetPackageReferencesAsync(projects)).ToList();
            await EnrichWithLicenseInformationAsync(packageReferences);
            
            _packageReferencePrinter.PrintPackageReferences(packageReferences);

            return packageReferences;
        }
        catch (Exception ex)
        {
            _logger.LogCritical($"Error occured while extracting licenses for '{_settings.TargetFile}'", ex);
            throw;
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
            _logger.LogInformation($"Trying to get projects from {targetFile}...");

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

    private async Task DownloadPackageReferencesFor(IEnumerable<Project> projects)
    {
        _logger.LogInformation("Downloading package-references for project(s)...");

        foreach (var project in projects)
            await _downloadPackageReferences.DownloadForProjectAsync(project).ConfigureAwait(false);
    }

    private async Task<IEnumerable<PackageReference>> GetPackageReferencesAsync(IEnumerable<Project> projects)
    {
        var packageReferences = new List<PackageReference>();

        _logger.LogInformation("Trying to get package-references for project(s)...");
        foreach (var project in projects)
        {
            var referencesFromProject = await GetPackageReferencesForProjectAsync(project);
            packageReferences.AddRange(referencesFromProject);
        }
        
        packageReferences = packageReferences
            .Distinct()
            .ToList();
        
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

    private async Task EnrichWithLicenseInformationAsync(IEnumerable<PackageReference> packageReferences)
    {
        _logger.LogInformation("Trying to get license information for package(s)...");

        foreach (var packageReference in packageReferences)
            await EnrichWithLicenseInformationForPackageReferenceAsync(packageReference);
    }

    private async Task EnrichWithLicenseInformationForPackageReferenceAsync(PackageReference packageReference)
    {
        try
        {
            _logger.LogDebug($"Trying to get license information for {packageReference}...");

            await _enrichPackageReferenceWithLicenseInformation.EnrichAsync(packageReference);
            _logger.LogDebug($"Found following license-type: '{packageReference.LicenseInformation}'");
        }
        catch (Exception ex)
        {
            throw new GetLicenseInformationFailedException(packageReference, ex);
        }
    }

    private void CleanUpTemporaryDirectory()
    {
        var temporaryDirectory = _provideTemporaryDirectories.GetRootDirectory();
        temporaryDirectory.Delete(true);
    }
}
