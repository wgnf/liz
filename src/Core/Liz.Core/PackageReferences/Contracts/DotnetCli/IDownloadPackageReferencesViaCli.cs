using Liz.Core.Projects.Contracts.Models;
using System.IO.Abstractions;

namespace Liz.Core.PackageReferences.Contracts.DotnetCli;

internal interface IDownloadPackageReferencesViaCli
{
    Task DownloadForProjectAsync(Project project, IDirectoryInfo targetDirectory);
}
