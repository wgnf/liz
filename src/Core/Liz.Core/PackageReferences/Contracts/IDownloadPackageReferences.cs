using Liz.Core.PackageReferences.Contracts.Models;
using Liz.Core.Projects.Contracts.Models;

namespace Liz.Core.PackageReferences.Contracts;

internal interface IDownloadPackageReferences
{
    Task DownloadAndEnrichAsync(IEnumerable<PackageReference> packageReferences);
}
