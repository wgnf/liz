using Liz.Core.Logging;
using Liz.Core.Logging.Contracts;
using Liz.Core.PackageReferences.Contracts;
using Liz.Core.PackageReferences.Contracts.DotnetCli;
using Liz.Core.Projects.Contracts.Models;
using Liz.Core.Utils.Contracts;
using System;
using System.IO.Abstractions;
using System.Threading.Tasks;

namespace Liz.Core.PackageReferences;

/*
 * NOTE:
 * Upside by using CLI-Tools here is that you don't have to implement everything for downloading stuff, like:
 * - finding and handling nuget-sources (local nuget.config, global nuget.config, v2 APIs, v3 APIs, retry-patterns, ...)
 * - finding, handling and using local caches
 * - ...
 */
internal sealed class DownloadPackageReferencesFacade : IDownloadPackageReferences
{
    private readonly IDownloadPackageReferencesViaDotnetCli _downloadPackageReferencesViaDotnetCli;
    private readonly IDownloadPackageReferencesViaNugetCli _downloadPackageReferencesViaNugetCli;
    private readonly ILogger _logger;
    private readonly IProvideTemporaryDirectories _provideTemporaryDirectories;

    public DownloadPackageReferencesFacade(
        IProvideTemporaryDirectories provideTemporaryDirectories,
        ILogger logger,
        IDownloadPackageReferencesViaDotnetCli downloadPackageReferencesViaDotnetCli,
        IDownloadPackageReferencesViaNugetCli downloadPackageReferencesViaNugetCli)
    {
        _provideTemporaryDirectories = provideTemporaryDirectories
                                     ?? throw new ArgumentNullException(nameof(provideTemporaryDirectories));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _downloadPackageReferencesViaDotnetCli = downloadPackageReferencesViaDotnetCli ??
                                                 throw new ArgumentNullException(
                                                     nameof(downloadPackageReferencesViaDotnetCli));
        _downloadPackageReferencesViaNugetCli = downloadPackageReferencesViaNugetCli ??
                                                throw new ArgumentNullException(
                                                    nameof(downloadPackageReferencesViaNugetCli));
    }

    public async Task DownloadForProjectAsync(Project project)
    {
        ArgumentNullException.ThrowIfNull(project);

        var targetDirectory = PrepareDownloadTargetDirectory();

        switch (project.FormatStyle)
        {
            case ProjectFormatStyle.SdkStyle:
                await _downloadPackageReferencesViaDotnetCli.DownloadForProjectAsync(project, targetDirectory);
                break;
            case ProjectFormatStyle.NonSdkStyle:
                await _downloadPackageReferencesViaNugetCli.DownloadForProjectAsync(project, targetDirectory);
                break;

            case ProjectFormatStyle.Unknown:
            default:
            {
                _logger.LogWarning($"Could not download package-references for project '{project.Name}', " +
                                   $"because it's style is '{project.FormatStyle}'! Skipping...");
                break;
            }
        }
    }

    private IDirectoryInfo PrepareDownloadTargetDirectory()
    {
        var temporaryDownloadDirectory = _provideTemporaryDirectories.GetDownloadDirectory();

        // make sure that the target directory is created
        temporaryDownloadDirectory.Create();

        return temporaryDownloadDirectory;
    }
}
