using Liz.Core.PackageReferences.Contracts.Models;
using System.Collections.Generic;

namespace Liz.Core.Utils.Contracts;

internal interface IPackageReferencePrinter
{
    void PrintPackageReferences(IEnumerable<PackageReference> packageReferences);
    void PrintPackageReferencesIssues(IEnumerable<PackageReference> packageReferences);
}
