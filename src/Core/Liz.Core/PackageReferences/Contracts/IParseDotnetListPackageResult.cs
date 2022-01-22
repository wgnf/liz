using JetBrains.Annotations;
using Liz.Core.PackageReferences.Contracts.Models;
using System.Collections.Generic;

namespace Liz.Core.PackageReferences.Contracts;

internal interface IParseDotnetListPackageResult
{
    IEnumerable<PackageReference> Parse([NotNull] string input);
}
