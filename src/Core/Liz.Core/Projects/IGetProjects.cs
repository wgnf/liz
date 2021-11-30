using JetBrains.Annotations;
using System.Collections.Generic;

namespace Liz.Core.Projects;

internal interface IGetProjects
{
    IEnumerable<Project> GetFromFile([NotNull] string targetFile);
}
