using Liz.Core.PackageReferences.Contracts.Models;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Liz.Core.Utils.Contracts;

internal interface IPackageReferencePrinter
{
    void PrintPackageReferences([NotNull] IEnumerable<PackageReference> packageReferences);
}
