using Liz.Core.CliTool.Contracts;
using Liz.Core.PackageReferences.Contracts.DotnetCli;
using Liz.Core.Projects.Contracts.Models;
using System.IO.Abstractions;

namespace Liz.Core.PackageReferences.DotnetCli;

/*
 * dotnet restore docs: https://docs.microsoft.com/en-us/dotnet/core/tools/dotnet-restore
 *
 * as both nuget-cli-restore and dotnet-cli-restore have different directory structures when restoring, they're both
 * kept in separate directories. When accessing the artifacts of a package, one would have to search in both of those
 * directories (see service 'GetDownloadedPackageReferenceArtifact')
 */
internal sealed class DownloadPackageReferencesViaDotnetCli : IDownloadPackageReferencesViaDotnetCli
{
    private readonly ICliToolExecutor _cliToolExecutor;

    public DownloadPackageReferencesViaDotnetCli(ICliToolExecutor cliToolExecutor)
    {
        _cliToolExecutor = cliToolExecutor ?? throw new ArgumentNullException(nameof(cliToolExecutor));
    }

    public async Task DownloadForProjectAsync(Project project, IDirectoryInfo targetDirectory)
    {
        ArgumentNullException.ThrowIfNull(project);
        ArgumentNullException.ThrowIfNull(targetDirectory);

        var projectFileName = project.File.FullName;
        var dotnetTargetDirectoryName = GetDotnetTargetDirectory(targetDirectory).FullName;

        var arguments = $"restore \"{projectFileName}\" --packages \"{dotnetTargetDirectoryName}\" --force";
        await _cliToolExecutor.ExecuteAsync("dotnet", arguments);
    }
    
    private static IDirectoryInfo GetDotnetTargetDirectory(IDirectoryInfo targetDirectory)
    {
        var dotnetTargetDirectory = targetDirectory.CreateSubdirectory("dotnet-dl");
        return dotnetTargetDirectory;
    }
}
