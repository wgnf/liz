using Liz.Core.PackageReferences.Contracts.Models;

namespace Liz.Core.PackageReferences.Contracts;

internal interface IDownloadPackageReferences
{
    Task DownloadAndEnrichAsync(IEnumerable<PackageReference> packageReferences);
}
