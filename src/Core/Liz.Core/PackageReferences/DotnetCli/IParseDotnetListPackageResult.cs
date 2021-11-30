using JetBrains.Annotations;
using System.Collections.Generic;

namespace DotnetNugetLicenses.Core.PackageReferences.DotnetCli;

internal interface IParseDotnetListPackageResult
{
    IEnumerable<PackageReference> Parse([NotNull] string input);
}
