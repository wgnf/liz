using JetBrains.Annotations;
using Liz.Core.CliTool.Contracts;
using Liz.Core.PackageReferences.Contracts;
using Liz.Core.PackageReferences.Contracts.Models;
using Liz.Core.Projects.Contracts.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Liz.Core.PackageReferences.DotnetCli;

internal sealed class GetPackageReferencesViaDotnetCli : IGetPackageReferencesViaDotnetCli
{
    private readonly ICliToolExecutor _cliToolExecutor;
    private readonly IParseDotnetListPackageResult _parseDotnetListPackageResult;

    public GetPackageReferencesViaDotnetCli(
        [NotNull] ICliToolExecutor cliToolExecutor,
        [NotNull] IParseDotnetListPackageResult parseDotnetListPackageResult)
    {
        _cliToolExecutor = cliToolExecutor ?? throw new ArgumentNullException(nameof(cliToolExecutor));
        _parseDotnetListPackageResult = parseDotnetListPackageResult ?? throw new ArgumentNullException(nameof(parseDotnetListPackageResult));
    }
    
    public async Task<IEnumerable<PackageReference>> GetFromProjectAsync(Project project, bool includeTransitive)
    {
        ArgumentNullException.ThrowIfNull(project);

        await RunDotNetRestore(project);
        var result = await RunDotNetListPackage(project, includeTransitive);
        var packageReferences = GetPackageReferencesFromListPackageResult(result);

        return packageReferences;
    }

    private async Task RunDotNetRestore(Project project)
    {
        await _cliToolExecutor.ExecuteAsync("dotnet", $"restore {project.File}");
    }

    private async Task<string> RunDotNetListPackage(Project project, bool includeTransitive)
    {
        // c.f.: https://docs.microsoft.com/de-de/dotnet/core/tools/dotnet-list-package
        var arguments = $"list \"{project.File}\" package {(includeTransitive ? "--include-transitive" : "")}";
        
        var result = await _cliToolExecutor.ExecuteWithResultAsync("dotnet", arguments);
        return result;
    }

    private IEnumerable<PackageReference> GetPackageReferencesFromListPackageResult(string result)
    {
        var packageReferences = _parseDotnetListPackageResult.Parse(result);
        return packageReferences;
    }
}
