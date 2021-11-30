using JetBrains.Annotations;
using Liz.Core.Projects;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Liz.Core.PackageReferences;

internal interface IGetPackageReferences
{
    Task<IEnumerable<PackageReference>> GetFromProjectAsync([NotNull] Project project, bool includeTransitive);
}
