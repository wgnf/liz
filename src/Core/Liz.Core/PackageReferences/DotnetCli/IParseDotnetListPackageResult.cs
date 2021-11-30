using JetBrains.Annotations;
using System.Collections.Generic;

namespace Liz.Core.PackageReferences.DotnetCli;

internal interface IParseDotnetListPackageResult
{
    IEnumerable<PackageReference> Parse([NotNull] string input);
}
