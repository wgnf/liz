using System.IO.Abstractions;

namespace Liz.Core.Utils.Contracts;

internal interface IProvideTemporaryDirectory
{
    IDirectoryInfo Get();
}
