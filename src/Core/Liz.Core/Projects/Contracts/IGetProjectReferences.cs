using Liz.Core.Projects.Contracts.Models;

namespace Liz.Core.Projects.Contracts;

internal interface IGetProjectReferences
{
    IEnumerable<ProjectReference> Get(Project project);
}
