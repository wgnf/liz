﻿using Liz.Core.Extract.Contracts;
using Liz.Core.License.Contracts;
using Liz.Core.License.Contracts.Exceptions;
using Liz.Core.Logging;
using Liz.Core.Logging.Contracts;
using Liz.Core.PackageReferences.Contracts;
using Liz.Core.PackageReferences.Contracts.Exceptions;
using Liz.Core.PackageReferences.Contracts.Models;
using Liz.Core.Preparation.Contracts;
using Liz.Core.Preparation.Contracts.Exceptions;
using Liz.Core.Progress;
using Liz.Core.Projects.Contracts;
using Liz.Core.Projects.Contracts.Exceptions;
using Liz.Core.Projects.Contracts.Models;
using Liz.Core.Result.Contracts;
using Liz.Core.Settings;
using Liz.Core.Utils.Contracts;

namespace Liz.Core.Extract;

internal sealed class ExtractLicenses : IExtractLicenses
{
    private readonly IDownloadPackageReferences _downloadPackageReferences;
    private readonly IEnrichPackageReferenceWithArtifactDirectory _enrichPackageReferenceWithArtifactDirectory;
    private readonly IEnrichPackageReferenceWithLicenseInformation _enrichPackageReferenceWithLicenseInformation;
    private readonly IGetPackageReferences _getPackageReferences;
    private readonly IGetProjects _getProjects;
    private readonly ILogger _logger;
    private readonly IEnumerable<IPreprocessor> _preprocessors;
    private readonly IProgressHandler? _progressHandler;
    private readonly IProvideTemporaryDirectories _provideTemporaryDirectories;
    private readonly IEnumerable<IResultProcessor> _resultProcessors;
    private readonly ExtractLicensesSettingsBase _settings;

    public ExtractLicenses(
        ExtractLicensesSettingsBase settings,
        ILogger logger,
        IProgressHandler? progressHandler,
        IGetProjects getProjects,
        IGetPackageReferences getPackageReferences,
        IEnrichPackageReferenceWithArtifactDirectory enrichPackageReferenceWithArtifactDirectory,
        IDownloadPackageReferences downloadPackageReferences,
        IEnrichPackageReferenceWithLicenseInformation enrichPackageReferenceWithLicenseInformation,
        IProvideTemporaryDirectories provideTemporaryDirectories,
        IEnumerable<IResultProcessor> resultProcessors,
        IEnumerable<IPreprocessor> preprocessors)
    {
        _settings = settings ?? throw new ArgumentNullException(nameof(settings));
        _getProjects = getProjects ?? throw new ArgumentNullException(nameof(getProjects));
        _getPackageReferences = getPackageReferences ?? throw new ArgumentNullException(nameof(getPackageReferences));
        _enrichPackageReferenceWithArtifactDirectory = enrichPackageReferenceWithArtifactDirectory ??
                                                       throw new ArgumentNullException(
                                                           nameof(enrichPackageReferenceWithArtifactDirectory));
        _downloadPackageReferences =
            downloadPackageReferences ?? throw new ArgumentNullException(nameof(downloadPackageReferences));
        _enrichPackageReferenceWithLicenseInformation = enrichPackageReferenceWithLicenseInformation ??
                                                        throw new ArgumentNullException(
                                                            nameof(enrichPackageReferenceWithLicenseInformation));
        _provideTemporaryDirectories = provideTemporaryDirectories
                                       ?? throw new ArgumentNullException(nameof(provideTemporaryDirectories));
        _resultProcessors = resultProcessors ?? throw new ArgumentNullException(nameof(resultProcessors));
        _preprocessors = preprocessors ?? throw new ArgumentNullException(nameof(preprocessors));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _progressHandler = progressHandler;
    }

    public async Task<IEnumerable<PackageReference>> ExtractAsync()
    {
        try
        {
            await PrepareAsync().ConfigureAwait(false);
            var projects = GetProjects(_settings.GetTargetFile()).ToList();

            var packageReferences = (await GetPackageReferencesAsync(projects).ConfigureAwait(false)).ToList();

            await EnrichWithArtifactDirectory(packageReferences).ConfigureAwait(false);
            await DownloadPackagesWithoutArtifactDirectory(packageReferences).ConfigureAwait(false);

            await EnrichWithLicenseInformationAsync(packageReferences).ConfigureAwait(false);

            _progressHandler?.FinishMainProcess();

            await ProcessResultsAsync(packageReferences).ConfigureAwait(false);
            return packageReferences;
        }
        catch (Exception ex)
        {
            _logger.LogCritical($"Error occured while extracting licenses for '{_settings.GetTargetFile()}'", ex);
            throw;
        }
        finally
        {
            CleanUpTemporaryDirectory();
        }
    }

    private async Task PrepareAsync()
    {
        try
        {
            /*
             * 6 main process steps:
             * - preparation
             * - get projects
             * - get package-references
             * - enrich with artifact-directory
             * - download packages without artifact-directory
             * - enrich with license info
             */
            _progressHandler?.InitializeMainProcess(6, "Preparing");
            _logger.LogInformation("Preparing...");

            foreach (var preprocessor in _preprocessors)
            {
                await preprocessor.PreprocessAsync().ConfigureAwait(false);
            }
        }
        catch (Exception exception)
        {
            throw new PreparationFailedException(exception);
        }
    }

    private IEnumerable<Project> GetProjects(string targetFile)
    {
        try
        {
            _progressHandler?.TickMainProcess("Get projects");
            _progressHandler?.InitializeNewSubProcess(1);
            _logger.LogInformation($"Trying to get projects from {targetFile}...");

            var projects = _getProjects.GetFromFile(targetFile).ToList();

            var foundProjectsLogString =
                string.Join(Environment.NewLine, projects.Select(project => $"\t- {project.File.FullName}"));
            _logger.LogDebug($"Found following projects for '{targetFile}':{Environment.NewLine}{foundProjectsLogString}");

            return projects;
        }
        catch (Exception exception)
        {
            throw new GetProjectsFailedException(targetFile, exception);
        }
    }

    private async Task<IEnumerable<PackageReference>> GetPackageReferencesAsync(IReadOnlyCollection<Project> projects)
    {
        var packageReferences = new List<PackageReference>();

        _logger.LogInformation("Trying to get package-references for project(s)...");

        _progressHandler?.TickMainProcess("Get package-references");
        _progressHandler?.InitializeNewSubProcess(projects.Count);

        foreach (var project in projects)
        {
            _progressHandler?.TickCurrentSubProcess($"Get package-references: {project.Name}");

            var referencesFromProject = await GetPackageReferencesForProjectAsync(project).ConfigureAwait(false);
            packageReferences.AddRange(referencesFromProject);
        }

        packageReferences = packageReferences
            .Distinct()
            .ToList();

        return packageReferences;
    }

    private async Task EnrichWithArtifactDirectory(IReadOnlyCollection<PackageReference> packageReferences)
    {
        _logger.LogInformation("Trying to get artifacts for package(s)...");

        _progressHandler?.TickMainProcess("Get artifacts");
        _progressHandler?.InitializeNewSubProcess(packageReferences.Count);

        foreach (var packageReference in packageReferences)
        {
            _progressHandler?.TickCurrentSubProcess($"Get artifact: {packageReference.Name}");
            await EnrichWithArtifactDirectoryForPackageReferenceAsync(packageReference).ConfigureAwait(false);
        }
    }

    private async Task DownloadPackagesWithoutArtifactDirectory(IEnumerable<PackageReference> packageReferences)
    {
        _logger.LogInformation("Downloading packages that are not in the cache...");

        _progressHandler?.TickMainProcess("Download packages");
        // we'll download everything in one go
        _progressHandler?.InitializeNewSubProcess(1);

        var packagesWithoutArtifactDirectory = packageReferences
            .Where(package => package.ArtifactDirectory == null);

        await _downloadPackageReferences.DownloadAndEnrichAsync(packagesWithoutArtifactDirectory).ConfigureAwait(false);
    }

    private async Task EnrichWithArtifactDirectoryForPackageReferenceAsync(PackageReference packageReference)
    {
        try
        {
            _logger.LogDebug($"Trying to get artifact directory for {packageReference}...");

            await _enrichPackageReferenceWithArtifactDirectory.EnrichAsync(packageReference).ConfigureAwait(false);

            _logger.LogDebug($"Found following artifact directory: '{packageReference.ArtifactDirectory}'");
        }
        catch (Exception exception)
        {
            throw new GetArtifactDirectoryFailedException(packageReference, exception);
        }
    }

    private async Task<IEnumerable<PackageReference>> GetPackageReferencesForProjectAsync(Project project)
    {
        try
        {
            _logger.LogDebug($"Trying to get package-references for project '{project.Name} ({project.File})'...");

            var packageReferences = (await _getPackageReferences
                .GetFromProjectAsync(project, _settings.IncludeTransitiveDependencies).ConfigureAwait(false)).ToList();

            var foundReferencesLogString = string.Join($"{Environment.NewLine}",
                packageReferences.Select(reference =>
                    $"\t- [{reference.TargetFramework}] {reference.Name} ({reference.Version})"));
            _logger.LogDebug(
                $"Found following package-references for '{project.Name} ({project.File})':{Environment.NewLine}{foundReferencesLogString}");

            return packageReferences;
        }
        catch (Exception exception)
        {
            throw new GetPackageReferencesFailedException(project, exception);
        }
    }

    private async Task EnrichWithLicenseInformationAsync(IReadOnlyCollection<PackageReference> packageReferences)
    {
        _logger.LogInformation("Trying to get license information for package(s)...");

        _progressHandler?.TickMainProcess("Get license information");
        _progressHandler?.InitializeNewSubProcess(packageReferences.Count);

        foreach (var packageReference in packageReferences)
        {
            _progressHandler?.TickCurrentSubProcess($"Get license information: {packageReference.Name}");

            await EnrichWithLicenseInformationForPackageReferenceAsync(packageReference).ConfigureAwait(false);
        }
    }

    private async Task EnrichWithLicenseInformationForPackageReferenceAsync(PackageReference packageReference)
    {
        try
        {
            _logger.LogDebug($"Trying to get license information for {packageReference}...");

            await _enrichPackageReferenceWithLicenseInformation.EnrichAsync(packageReference).ConfigureAwait(false);
            _logger.LogDebug($"Found following license-type: '{packageReference.LicenseInformation}'");
        }
        catch (Exception exception)
        {
            throw new GetLicenseInformationFailedException(packageReference, exception);
        }
    }

    private async Task ProcessResultsAsync(IReadOnlyCollection<PackageReference> packageReferences)
    {
        _logger.LogInformation("Processing results...");

        foreach (var resultProcessor in _resultProcessors)
        {
            _logger.LogDebug($"Processing with '{resultProcessor.GetType().Name}'");

            await resultProcessor.ProcessResultsAsync(packageReferences).ConfigureAwait(false);
        }
    }

    private void CleanUpTemporaryDirectory()
    {
        var temporaryDirectory = _provideTemporaryDirectories.GetRootDirectory();

        if (!temporaryDirectory.Exists)
        {
            return;
        }

        temporaryDirectory.Delete(true);
    }
}
