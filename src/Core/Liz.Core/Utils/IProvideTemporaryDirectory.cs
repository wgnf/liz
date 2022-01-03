using System.IO.Abstractions;

namespace Liz.Core.Utils;

internal interface IProvideTemporaryDirectory
{
    IDirectoryInfo Get();
}
