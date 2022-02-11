using Liz.Core.PackageReferences.Contracts.Models;
using Liz.Core.Projects.Contracts.Models;

namespace Liz.Core.PackageReferences.Contracts;

internal interface IGetPackageReferences
{
    Task<IEnumerable<PackageReference>> GetFromProjectAsync(Project project, bool includeTransitive);
}
