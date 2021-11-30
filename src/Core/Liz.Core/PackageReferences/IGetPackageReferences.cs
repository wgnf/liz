using DotnetNugetLicenses.Core.Projects;
using JetBrains.Annotations;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DotnetNugetLicenses.Core.PackageReferences;

internal interface IGetPackageReferences
{
    Task<IEnumerable<PackageReference>> GetFromProjectAsync([NotNull] Project project, bool includeTransitive);
}
