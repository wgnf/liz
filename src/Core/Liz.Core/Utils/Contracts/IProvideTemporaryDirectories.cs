using System.IO.Abstractions;

namespace Liz.Core.Utils.Contracts;

internal interface IProvideTemporaryDirectories
{
    IDirectoryInfo GetRootDirectory();
    IDirectoryInfo GetDownloadDirectory();
}
