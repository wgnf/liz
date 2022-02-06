using JetBrains.Annotations;
using Liz.Core.Projects.Contracts.Models;
using System.Threading.Tasks;

namespace Liz.Core.PackageReferences.Contracts;

internal interface IDownloadPackageReferences
{
    Task DownloadForProjectAsync([NotNull] Project project);
}
