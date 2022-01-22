using JetBrains.Annotations;
using Liz.Core.PackageReferences.Contracts.Models;
using Liz.Core.Projects.Contracts.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Liz.Core.PackageReferences.Contracts;

internal interface IGetPackageReferences
{
    Task<IEnumerable<PackageReference>> GetFromProjectAsync([NotNull] Project project, bool includeTransitive);
}
