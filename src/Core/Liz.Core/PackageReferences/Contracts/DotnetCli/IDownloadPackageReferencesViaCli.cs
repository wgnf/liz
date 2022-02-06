using Liz.Core.Projects.Contracts.Models;
using System.Diagnostics.CodeAnalysis;
using System.IO.Abstractions;
using System.Threading.Tasks;

namespace Liz.Core.PackageReferences.Contracts.DotnetCli;

internal interface IDownloadPackageReferencesViaCli
{
    Task DownloadForProjectAsync([NotNull] Project project, [NotNull] IDirectoryInfo targetDirectory);
}
