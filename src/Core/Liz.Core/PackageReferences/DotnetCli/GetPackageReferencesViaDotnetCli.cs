using Liz.Core.CliTool.Contracts;
using Liz.Core.PackageReferences.Contracts;
using Liz.Core.PackageReferences.Contracts.DotnetCli;
using Liz.Core.PackageReferences.Contracts.Models;
using Liz.Core.Projects.Contracts;
using Liz.Core.Projects.Contracts.Models;

namespace Liz.Core.PackageReferences.DotnetCli;

internal sealed class GetPackageReferencesViaDotnetCli : IGetPackageReferencesViaDotnetCli
{
    private readonly ICliToolExecutor _cliToolExecutor;
    private readonly IParseDotnetListPackageResult _parseDotnetListPackageResult;
    private readonly IGetProjectReferences _getProjectReferences;

    public GetPackageReferencesViaDotnetCli(
        ICliToolExecutor cliToolExecutor,
        IParseDotnetListPackageResult parseDotnetListPackageResult,
        IGetProjectReferences getProjectReferences)
    {
        _cliToolExecutor = cliToolExecutor ?? throw new ArgumentNullException(nameof(cliToolExecutor));
        _parseDotnetListPackageResult = parseDotnetListPackageResult ?? throw new ArgumentNullException(nameof(parseDotnetListPackageResult));
        _getProjectReferences = getProjectReferences ?? throw new ArgumentNullException(nameof(getProjectReferences));
    }
    
    public async Task<IEnumerable<PackageReference>> GetFromProjectAsync(Project project, bool includeTransitive)
    {
        if (project == null) throw new ArgumentNullException(nameof(project));

        await RunDotNetRestore(project).ConfigureAwait(false);
        var result = await RunDotNetListPackage(project, includeTransitive).ConfigureAwait(false);
        var packageReferences = GetPackageReferencesFromListPackageResult(result).ToList();

        if (!includeTransitive) return packageReferences;

        RemoveProjectReferencePackages(project, packageReferences);

        return packageReferences;
    }

    private async Task RunDotNetRestore(Project project)
    {
        await _cliToolExecutor.ExecuteAsync("dotnet", $"restore {project.File}").ConfigureAwait(false);
    }

    private async Task<string> RunDotNetListPackage(Project project, bool includeTransitive)
    {
        // c.f.: https://docs.microsoft.com/de-de/dotnet/core/tools/dotnet-list-package
        var arguments = $"list \"{project.File}\" package {(includeTransitive ? "--include-transitive" : "")}";

        var result = await _cliToolExecutor.ExecuteWithResultAsync("dotnet", arguments).ConfigureAwait(false);
        return result;
    }

    private IEnumerable<PackageReference> GetPackageReferencesFromListPackageResult(string result)
    {
        var packageReferences = _parseDotnetListPackageResult.Parse(result);
        return packageReferences;
    }

    private void RemoveProjectReferencePackages(Project project, List<PackageReference> packageReferences)
    {
        var projectReferenceNames = _getProjectReferences.GetProjectReferenceNames(project);

        foreach (var projectReferenceName in projectReferenceNames)
            packageReferences.RemoveAll(packageReference => packageReference.Name == projectReferenceName);
    }
}
