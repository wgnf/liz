using DotNet.Globbing;
using Liz.Core.Logging;
using Liz.Core.Logging.Contracts;
using Liz.Core.PackageReferences.Contracts;
using Liz.Core.PackageReferences.Contracts.DotnetCli;
using Liz.Core.PackageReferences.Contracts.Models;
using Liz.Core.PackageReferences.Contracts.NuGetCli;
using Liz.Core.Projects.Contracts.Models;
using Liz.Core.Settings;

namespace Liz.Core.PackageReferences;

internal sealed class GetPackageReferencesFacade : IGetPackageReferences
{
    private readonly IGetPackageReferencesViaDotnetCli _getPackageReferencesViaDotnetCli;
    private readonly IGetPackageReferencesViaPackagesConfig _getPackageReferencesViaPackagesConfig;
    private readonly ExtractLicensesSettingsBase _settings;
    private readonly ILogger _logger;

    public GetPackageReferencesFacade(
        ExtractLicensesSettingsBase settings,
        ILogger logger, 
        IGetPackageReferencesViaDotnetCli getPackageReferencesViaDotnetCli,
        IGetPackageReferencesViaPackagesConfig getPackageReferencesViaPackagesConfig)
    {
        _settings = settings ?? throw new ArgumentNullException(nameof(settings));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _getPackageReferencesViaDotnetCli = getPackageReferencesViaDotnetCli 
                                            ?? throw new ArgumentNullException(nameof(getPackageReferencesViaDotnetCli));
        _getPackageReferencesViaPackagesConfig = getPackageReferencesViaPackagesConfig 
                                           ?? throw new ArgumentNullException(nameof(getPackageReferencesViaPackagesConfig));
    }

    public async Task<IEnumerable<PackageReference>> GetFromProjectAsync(Project project, bool includeTransitive)
    {
        if (project == null)
        {
            throw new ArgumentNullException(nameof(project));
        }

        if (!project.File.Exists)
        {
            throw new FileNotFoundException($"Project file '{project.File}' does not exist");
        }

        var packages = await (project.FormatStyle switch
        {
            ProjectFormatStyle.Unknown => throw new ArgumentException(
                $"Cannot determine Dependencies for project '{project.Name}' " +
                $"with Format-Style '{project.FormatStyle}'", nameof(project)),
            
            ProjectFormatStyle.SdkStyle => GetFromSdkStyle(project, includeTransitive),
            ProjectFormatStyle.NonSdkStyle => GetFromNonSdkStyle(project, includeTransitive),
            
            _ => throw new NotSupportedException(
                $"Format-Style '{project.FormatStyle}' for project '{project.Name}' is not supported")
        }).ConfigureAwait(false);

        packages = HandleExclusions(packages);
        return packages;
    }

    private async Task<IEnumerable<PackageReference>> GetFromSdkStyle(Project project, bool includeTransitive)
    {
        _logger.LogDebug($"Project '{project.Name} ({project.File})' " +
                         "appears to be in SDK-Style-Format. Getting package-references via dotnet cli...");

        var packageReferences = await _getPackageReferencesViaDotnetCli
            .GetFromProjectAsync(project, includeTransitive)
            .ConfigureAwait(false);
        return packageReferences;
    }

    private async Task<IEnumerable<PackageReference>> GetFromNonSdkStyle(Project project, bool includeTransitive)
    {
        _logger.LogDebug($"Project '{project.Name} ({project.File})' " +
                         "appears to be in non-SDK-Style-Format. Getting package-references via packages.config...");

        var packageReferences = await _getPackageReferencesViaPackagesConfig
            .GetFromProjectAsync(project, includeTransitive)
            .ConfigureAwait(false);
        return packageReferences;
    }

    private IEnumerable<PackageReference> HandleExclusions(IEnumerable<PackageReference> packageReferences)
    {
        // this basically filters out any package which matches a given exclusion-glob
        var exclusionGlobs = _settings.PackageExclusionGlobs;
        var filteredPackages = packageReferences.Where(packageReference =>
        {
            return !exclusionGlobs.Exists(glob =>
            {
                var globPattern = Glob.Parse(glob);
                var isMatch = globPattern.IsMatch(packageReference.Name);
                return isMatch;
            });
        });

        return filteredPackages;
    }
}
