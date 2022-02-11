using Liz.Core.PackageReferences.Contracts.Models;

namespace Liz.Core.PackageReferences.Contracts;

internal interface IParseDotnetListPackageResult
{
    IEnumerable<PackageReference> Parse(string input);
}
