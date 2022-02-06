using Liz.Core.Projects.Contracts.Models;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Liz.Core.Projects.Contracts;

internal interface IGetProjects
{
    IEnumerable<Project> GetFromFile([NotNull] string targetFile);
}
