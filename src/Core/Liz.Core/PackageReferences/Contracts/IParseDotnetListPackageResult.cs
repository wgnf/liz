using Liz.Core.PackageReferences.Contracts.Models;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Liz.Core.PackageReferences.Contracts;

internal interface IParseDotnetListPackageResult
{
    IEnumerable<PackageReference> Parse([NotNull] string input);
}
