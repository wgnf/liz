using System.IO.Abstractions;

namespace Liz.Core.PackageReferences.Contracts.DotnetCli;

internal interface IDownloadPackageReferencesViaDotnetCli
{
    Task DownloadAsync(IFileInfo targetProjectFile, IDirectoryInfo targetDirectory);
}
