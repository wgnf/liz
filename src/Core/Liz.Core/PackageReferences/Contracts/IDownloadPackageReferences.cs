using Liz.Core.Projects.Contracts.Models;

namespace Liz.Core.PackageReferences.Contracts;

internal interface IDownloadPackageReferences
{
    Task DownloadForProjectAsync(Project project);
}
