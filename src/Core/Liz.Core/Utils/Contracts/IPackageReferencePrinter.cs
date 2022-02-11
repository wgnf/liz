using Liz.Core.PackageReferences.Contracts.Models;

namespace Liz.Core.Utils.Contracts;

internal interface IPackageReferencePrinter
{
    void PrintPackageReferences(IEnumerable<PackageReference> packageReferences);
    void PrintPackageReferencesIssues(IEnumerable<PackageReference> packageReferences);
}
