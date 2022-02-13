using Liz.Core.PackageReferences.Contracts.Models;
using System.IO.Abstractions;

namespace Liz.Core.PackageReferences.Contracts.NuGetCli;

internal interface IParsePackagesConfigFile
{
    IEnumerable<PackageReference> GetPackageReferences(IFileInfo packagesConfigFile);
}
