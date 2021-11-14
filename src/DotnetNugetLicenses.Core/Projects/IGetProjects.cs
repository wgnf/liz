using JetBrains.Annotations;
using System.Collections.Generic;
using System.IO.Abstractions;

namespace DotnetNugetLicenses.Core.Projects;

internal interface IGetProjects
{
    IEnumerable<Project> GetFromFile([NotNull] string targetFile);
}
