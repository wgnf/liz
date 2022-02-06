using JetBrains.Annotations;
using Liz.Core.CliTool.Contracts;
using Liz.Core.PackageReferences.Contracts.DotnetCli;
using Liz.Core.Projects.Contracts.Models;
using System;
using System.IO;
using System.IO.Abstractions;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;

namespace Liz.Core.PackageReferences.DotnetCli;

/*
 * NOTE:
 * - general docs: https://docs.microsoft.com/en-us/nuget/consume-packages/package-restore#restore-using-the-nugetexe-cli
 * - nuget restore specific docs: https://docs.microsoft.com/en-us/nuget/reference/cli-reference/cli-ref-restore
 *
 * as both nuget-cli-restore and dotnet-cli-restore have different directory structures when restoring, they're both
 * kept in separate directories. When accessing the artifacts of a package, one would have to search in both of those
 * directories (see service 'GetDownloadedPackageReferenceArtifact')
 *
 * Additionally for nuget-restore we unzip the .nupkg files (because that is not done automatically and it contains
 * interesting files that we want)
 */ 
internal sealed class DownloadPackageReferencesViaNugetCli : IDownloadPackageReferencesViaNugetCli
{
    private readonly ICliToolExecutor _cliToolExecutor;

    public DownloadPackageReferencesViaNugetCli([NotNull] ICliToolExecutor cliToolExecutor)
    {
        _cliToolExecutor = cliToolExecutor ?? throw new ArgumentNullException(nameof(cliToolExecutor));
    }
    
    public async Task DownloadForProjectAsync(Project project, IDirectoryInfo targetDirectory)
    {
        ArgumentNullException.ThrowIfNull(project);
        ArgumentNullException.ThrowIfNull(targetDirectory);

        var projectFileName = project.File.FullName;
        var nugetTargetDirectory = GetNugetTargetDirectory(targetDirectory);
        
        await DownloadPackageReferencesAsync(projectFileName, nugetTargetDirectory);
        UnzipNugetPackages(nugetTargetDirectory);
    }

    private async Task DownloadPackageReferencesAsync(string projectFileName, IFileSystemInfo targetDirectory)
    {
        var targetDirectoryName = targetDirectory.FullName;
        
        var arguments = $"restore \"{projectFileName}\"  -PackagesDirectory \"{targetDirectoryName}\" -Force -NonInteractive";
        await _cliToolExecutor.ExecuteAsync("nuget", arguments);
    }
    
    private static IDirectoryInfo GetNugetTargetDirectory(IDirectoryInfo targetDirectory)
    {
        var nugetTargetDirectory = targetDirectory.CreateSubdirectory("nuget-dl");
        return nugetTargetDirectory;
    }

    private static void UnzipNugetPackages(IDirectoryInfo targetDirectory)
    {
        var nugetPackageFiles = targetDirectory
            .EnumerateFiles(
                "*.nupkg",
                new EnumerationOptions
                {
                    MatchCasing = MatchCasing.CaseInsensitive,
                    RecurseSubdirectories = true,
                    IgnoreInaccessible = true,
                    MatchType = MatchType.Simple
                })
            .ToList();
        
        nugetPackageFiles.ForEach(UnzipNugetPackage);
    }

    private static void UnzipNugetPackage(IFileInfo fileInfo)
    {
        var fileStream = fileInfo.OpenRead();
        var zipArchive = new ZipArchive(fileStream);
        
        zipArchive.ExtractToDirectory(fileInfo.Directory.FullName, true);
    }
}
