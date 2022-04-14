using Liz.Core.Extract.Contracts;
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
    private readonly IGetPackageReferences _getPackageReferences;
    private readonly IEnrichPackageReferenceWithLicenseInformation _enrichPackageReferenceWithLicenseInformation;
    private readonly IProvideTemporaryDirectories _provideTemporaryDirectories;
    private readonly IDownloadPackageReferences _downloadPackageReferences;
    private readonly IEnumerable<IResultProcessor> _resultProcessors;
    private readonly IEnumerable<IPreprocessor> _preprocessors;
    private readonly IGetProjects _getProjects;
    private readonly ILogger _logger;
    private readonly IProgressHandler? _progressHandler;
    private readonly ExtractLicensesSettingsBase _settings;

    public ExtractLicenses(
        ExtractLicensesSettingsBase settings,
        ILogger logger,
        IProgressHandler? progressHandler,
        IGetProjects getProjects,
        IGetPackageReferences getPackageReferences,
        IEnrichPackageReferenceWithLicenseInformation enrichPackageReferenceWithLicenseInformation,
        IProvideTemporaryDirectories provideTemporaryDirectories,
        IDownloadPackageReferences downloadPackageReferences,
        IEnumerable<IResultProcessor> resultProcessors,
        IEnumerable<IPreprocessor> preprocessors)
    {
        _settings = settings ?? throw new ArgumentNullException(nameof(settings));
        _getProjects = getProjects ?? throw new ArgumentNullException(nameof(getProjects));
        _getPackageReferences = getPackageReferences ?? throw new ArgumentNullException(nameof(getPackageReferences));
        _enrichPackageReferenceWithLicenseInformation = enrichPackageReferenceWithLicenseInformation ?? 
                                                        throw new ArgumentNullException(nameof(enrichPackageReferenceWithLicenseInformation));
        _provideTemporaryDirectories = provideTemporaryDirectories 
                                     ?? throw new ArgumentNullException(nameof(provideTemporaryDirectories));
        _downloadPackageReferences = downloadPackageReferences ?? throw new ArgumentNullException(nameof(downloadPackageReferences));
        _resultProcessors = resultProcessors ?? throw new ArgumentNullException(nameof(resultProcessors));
        _preprocessors = preprocessors ?? throw new ArgumentNullException(nameof(preprocessors));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _progressHandler = progressHandler;
    }

    public async Task<IEnumerable<PackageReference>> ExtractAsync()
    {
        try
        {
            await PrepareAsync();
            var projects = GetProjects(_settings.GetTargetFile()!).ToList();

            await DownloadPackageReferencesFor(projects);
            
            var packageReferences = (await GetPackageReferencesAsync(projects)).ToList();
            await EnrichWithLicenseInformationAsync(packageReferences);

            packageReferences = packageReferences
                /*
                 * this gets rid of all the references that have NO data in the license-information what so ever
                 * (which basically are the internal project-references for now)
                 */
                .Where(packageReference => !packageReference.LicenseInformation.IsEmpty())
                .ToList();
            
            _progressHandler?.FinishMainProcess();

            await ProcessResultsAsync(packageReferences);
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
             * 5 main process steps:
             * - pre process
             * - get projects
             * - download package-references
             * - get package-references
             * - enrich license info
             */
            _progressHandler?.InitializeMainProcess(5, "Preparing");
            _logger.LogInformation("Preparing...");

            foreach (var preprocessor in _preprocessors)
                await preprocessor.PreprocessAsync().ConfigureAwait(false);
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

            var foundProjectsLogString = string.Join(Environment.NewLine, projects.Select(project => $"\t- {project.File.FullName}"));
            _logger.LogDebug($"Found following projects for '{targetFile}':{Environment.NewLine}{foundProjectsLogString}");

            return projects;
        }
        catch (Exception exception)
        {
            throw new GetProjectsFailedException(targetFile, exception);
        }
    }

    private async Task DownloadPackageReferencesFor(IEnumerable<Project> projects)
    {
        var projectsList = projects.ToList();
        
        _logger.LogInformation("Downloading package-references for project(s)...");
     
        _progressHandler?.TickMainProcess("Download package-references");
        _progressHandler?.InitializeNewSubProcess(projectsList.Count);
        
        foreach (var project in projectsList)
        {
            _progressHandler?.TickCurrentSubProcess($"Download package-references: {project.Name}");
            
            await _downloadPackageReferences.DownloadForProjectAsync(project).ConfigureAwait(false);
        }
    }

    private async Task<IEnumerable<PackageReference>> GetPackageReferencesAsync(IEnumerable<Project> projects)
    {
        var packageReferences = new List<PackageReference>();
        var projectsList = projects.ToList();
        
        _logger.LogInformation("Trying to get package-references for project(s)...");

        _progressHandler?.TickMainProcess("Get package-references");
        _progressHandler?.InitializeNewSubProcess(projectsList.Count);
        
        foreach (var project in projectsList)
        {
            _progressHandler?.TickCurrentSubProcess($"Get package-references: {project.Name}");
            
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

    private async Task EnrichWithLicenseInformationAsync(IEnumerable<PackageReference> packageReferences)
    {
        var packageReferencesList = packageReferences.ToList();
        
        _logger.LogInformation("Trying to get license information for package(s)...");
        
        _progressHandler?.TickMainProcess("Get license information");
        _progressHandler?.InitializeNewSubProcess(packageReferencesList.Count);

        foreach (var packageReference in packageReferencesList)
        {
            _progressHandler?.TickCurrentSubProcess($"Get license information: {packageReference.Name}");
            
            await EnrichWithLicenseInformationForPackageReferenceAsync(packageReference);
        }
    }

    private async Task EnrichWithLicenseInformationForPackageReferenceAsync(PackageReference packageReference)
    {
        try
        {
            _logger.LogDebug($"Trying to get license information for {packageReference}...");

            await _enrichPackageReferenceWithLicenseInformation.EnrichAsync(packageReference);
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
        temporaryDirectory.Delete(true);
    }
}
