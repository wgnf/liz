using Liz.Core.Logging;
using Liz.Core.Logging.Contracts;
using Liz.Core.PackageReferences.Contracts;
using Liz.Core.PackageReferences.Contracts.DotnetCli;
using Liz.Core.PackageReferences.Contracts.Models;
using Liz.Core.Utils.Contracts;
using System.IO.Abstractions;
using System.Text;

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
    private readonly IFileSystem _fileSystem;
    private readonly ILogger _logger;
    private readonly IProvideTemporaryDirectories _provideTemporaryDirectories;

    public DownloadPackageReferencesFacade(
        IProvideTemporaryDirectories provideTemporaryDirectories,
        ILogger logger,
        IDownloadPackageReferencesViaDotnetCli downloadPackageReferencesViaDotnetCli,
        IFileSystem fileSystem)
    {
        _provideTemporaryDirectories = provideTemporaryDirectories
                                     ?? throw new ArgumentNullException(nameof(provideTemporaryDirectories));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _downloadPackageReferencesViaDotnetCli = downloadPackageReferencesViaDotnetCli ??
                                                 throw new ArgumentNullException(
                                                     nameof(downloadPackageReferencesViaDotnetCli));
        _fileSystem = fileSystem ?? throw new ArgumentNullException(nameof(fileSystem));
    }

    public async Task DownloadAndEnrichAsync(IEnumerable<PackageReference> packageReferences)
    {
        if (packageReferences == null) throw new ArgumentNullException(nameof(packageReferences));

        var packageReferencesList = packageReferences.ToList();

        if (!packageReferencesList.Any()) return;
        
        _logger.LogDebug("Downloading packages...");

        var targetDirectory = PrepareDownloadTargetDirectory();
        var dummyProjects = PrepareDummyProjectFiles(targetDirectory, packageReferencesList);

        foreach (var dummyProject in dummyProjects)
        {
            _logger.LogDebug($"Downloading packages of {dummyProject}...");
            await _downloadPackageReferencesViaDotnetCli.DownloadAsync(dummyProject, targetDirectory).ConfigureAwait(false);
        }

        EnrichWithDownloadedArtifactDirectories(targetDirectory, packageReferencesList);
    }

    private IDirectoryInfo PrepareDownloadTargetDirectory()
    {
        var temporaryDownloadDirectory = _provideTemporaryDirectories.GetDownloadDirectory();

        // make sure that the target directory is created
        temporaryDownloadDirectory.Create();

        return temporaryDownloadDirectory;
    }

    /*
     * NOTE:
     * we have to group using the TargetFramework here, because some packages might not support any upper-level frameworks,
     * so to just be save, we'll use the actual TargetFramework here and create a project for each one of them
     *
     * on the other hand, determining the highest "capture all" framework would mean some manual work that I want to
     * avoid for now
     */
    private IEnumerable<IFileInfo> PrepareDummyProjectFiles(
        IFileSystemInfo targetDirectory,
        IEnumerable<PackageReference> packageReferences)
    {
        var packageReferencesGroupedByFramework = packageReferences.GroupBy(package => package.TargetFramework);

        var dummyProjects = packageReferencesGroupedByFramework
            .Select(packageReferenceGroup => 
                CreateDummyProjectForFramework(targetDirectory, packageReferenceGroup.Key, packageReferenceGroup));
        return dummyProjects;
    }

    private IFileInfo CreateDummyProjectForFramework(
        IFileSystemInfo targetDirectory,
        string framework, 
        IEnumerable<PackageReference> packageReferences)
    {
        var targetProjectFile = _fileSystem.Path.Combine(
            targetDirectory.FullName, 
            "projects", 
            $"ProjectToDownload-{framework}.csproj");
        var targetProjectFileInfo = _fileSystem.FileInfo.FromFileName(targetProjectFile);
        
        // make sure that the target-directory actually exists
        targetProjectFileInfo.Directory.Create();

        var contentBuilder = new StringBuilder();
        
        contentBuilder.AppendLine("<Project Sdk=\"Microsoft.NET.Sdk\">");
        contentBuilder.AppendLine("\t<PropertyGroup>");
        contentBuilder.AppendLine($"\t\t<TargetFramework>{framework}</TargetFramework>");
        contentBuilder.AppendLine("\t</PropertyGroup>");

        contentBuilder.AppendLine("\t<ItemGroup>");
        
        foreach (var packageReference in packageReferences)
            contentBuilder.AppendLine(
                $"\t\t<PackageReference Include=\"{packageReference.Name}\" Version=\"{packageReference.Version}\" />");
        
        contentBuilder.AppendLine("\t</ItemGroup>");
        contentBuilder.AppendLine("</Project>");
        
        _fileSystem.File.WriteAllText(targetProjectFile, contentBuilder.ToString());

        return targetProjectFileInfo;
    }
    
    private void EnrichWithDownloadedArtifactDirectories(
        IFileSystemInfo targetDirectory, 
        List<PackageReference> packageReferences)
    {
        foreach (var packageReference in packageReferences)
        {
            var candidateArtifactDirectory = _fileSystem.Path.Combine(
                targetDirectory.FullName,
                packageReference.Name.ToLower(),
                packageReference.Version.ToLower());

            if (!_fileSystem.Directory.Exists(candidateArtifactDirectory)) continue;

            packageReference.ArtifactDirectory = candidateArtifactDirectory;
        }
    }
}
