using System.IO.Abstractions;
using System.Threading.Tasks;

namespace Liz.Core.PackageReferences;

internal interface IDownloadPackageReference
{
    Task<IDirectoryInfo> DownloadAsync(PackageReference packageReference);
}
