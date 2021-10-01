using DotnetNugetLicenses.Core.Models;
using JetBrains.Annotations;
using System.Collections.Generic;
using System.IO.Abstractions;

namespace DotnetNugetLicenses.Core.Contracts.Services
{
    public interface IGetProjects
    {
        IEnumerable<Project> GetFromFile([NotNull] IFileInfo targetFile);
    }
}
