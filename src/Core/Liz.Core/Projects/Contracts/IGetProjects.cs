using Liz.Core.Projects.Contracts.Models;
using System.Collections.Generic;

namespace Liz.Core.Projects.Contracts;

internal interface IGetProjects
{
    IEnumerable<Project> GetFromFile(string targetFile);
}
