using DotnetNugetLicenses.Core.Projects;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DotnetNugetLicenses.Core.PackageReferences.DotnetCli;

internal sealed class GetPackageReferencesViaDotnetCli : IGetPackageReferencesViaDotnetCli
{
    public async Task<IEnumerable<PackageReference>> GetFromProjectAsync(Project project, bool includeTransitive)
    {
        // dotnet restore
        // dotnet list package (--include transitive)

        throw new NotImplementedException();
    }
}
