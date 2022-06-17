using Liz.Core.CliTool.Contracts;
using Liz.Core.PackageReferences.Contracts.DotnetCli;
using System.IO.Abstractions;

namespace Liz.Core.PackageReferences.DotnetCli;

/*
 * dotnet restore docs: https://docs.microsoft.com/en-us/dotnet/core/tools/dotnet-restore
 */
internal sealed class DownloadPackageReferencesViaDotnetCli : IDownloadPackageReferencesViaDotnetCli
{
    private readonly ICliToolExecutor _cliToolExecutor;

    public DownloadPackageReferencesViaDotnetCli(ICliToolExecutor cliToolExecutor)
    {
        _cliToolExecutor = cliToolExecutor ?? throw new ArgumentNullException(nameof(cliToolExecutor));
    }

    public async Task DownloadAsync(IFileInfo targetProjectFile, IDirectoryInfo targetDirectory)
    {
        if (targetProjectFile == null) throw new ArgumentNullException(nameof(targetProjectFile));
        if (targetDirectory == null) throw new ArgumentNullException(nameof(targetDirectory));

        var projectFileName = targetProjectFile.FullName;
        var targetDirectoryName = targetDirectory.FullName;

        var arguments = $"restore \"{projectFileName}\" --packages \"{targetDirectoryName}\" --force";
        await _cliToolExecutor.ExecuteAsync("dotnet", arguments).ConfigureAwait(false);
    }
}
