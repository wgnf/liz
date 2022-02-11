using Liz.Core.Projects.Contracts.Models;

namespace Liz.Core.Projects.Contracts;

internal interface IGetProjects
{
    IEnumerable<Project> GetFromFile(string targetFile);
}
