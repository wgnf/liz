using Liz.Core.PackageReferences.Contracts.Models;
using System.IO.Abstractions;
using System.Threading.Tasks;

namespace Liz.Core.PackageReferences.Contracts;

internal interface IDownloadPackageReference
{
    Task<IDirectoryInfo> DownloadAsync(PackageReference packageReference);
}
