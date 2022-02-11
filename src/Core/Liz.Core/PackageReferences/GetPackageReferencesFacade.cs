using Liz.Core.Logging;
using Liz.Core.Logging.Contracts;
using Liz.Core.PackageReferences.Contracts;
using Liz.Core.PackageReferences.Contracts.DotnetCli;
using Liz.Core.PackageReferences.Contracts.Models;
using Liz.Core.Projects.Contracts.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Liz.Core.PackageReferences;

internal sealed class GetPackageReferencesFacade : IGetPackageReferences
{
    private readonly IGetPackageReferencesViaDotnetCli _getPackageReferencesViaDotnetCli;
    private readonly ILogger _logger;

    public GetPackageReferencesFacade(ILogger logger, IGetPackageReferencesViaDotnetCli getPackageReferencesViaDotnetCli)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _getPackageReferencesViaDotnetCli = getPackageReferencesViaDotnetCli ??
                                            throw new ArgumentNullException(nameof(getPackageReferencesViaDotnetCli));
    }

    public Task<IEnumerable<PackageReference>> GetFromProjectAsync(Project project, bool includeTransitive)
    {
        ArgumentNullException.ThrowIfNull(project);
        if (!project.File.Exists)
            throw new FileNotFoundException($"Project file '{project.File}' does not exist");

        return project.FormatStyle switch
        {
            ProjectFormatStyle.Unknown => throw new ArgumentException(
                $"Cannot determine Dependencies for project '{project.Name}' " +
                $"with Format-Style '{project.FormatStyle}'", nameof(project)),
            
            ProjectFormatStyle.SdkStyle => GetFromSdkStyle(project, includeTransitive),
            ProjectFormatStyle.NonSdkStyle => GetFromNonSdkStyle(project),
            
            _ => throw new NotSupportedException(
                $"Format-Style '{project.FormatStyle}' for project '{project.Name}' is not supported")
        };
    }

    private async Task<IEnumerable<PackageReference>> GetFromSdkStyle(Project project, bool includeTransitive)
    {
        _logger.LogDebug($"Project '{project.Name} ({project.File})' " +
                         "appears to be in SDK-Style-Format. Getting package-references via dotnet cli...");
        return await _getPackageReferencesViaDotnetCli.GetFromProjectAsync(project, includeTransitive);
    }

    private Task<IEnumerable<PackageReference>> GetFromNonSdkStyle(Project project)
    {
        _logger.LogDebug($"Project '{project.Name} ({project.File})' " +
                         "appears to be in non-SDK-Style-Format. Getting package-references via packages.config...");

        _logger.LogWarning($"Project '{project.Name} ({project.File})' " +
                           "is in the non-SDK-Style-Format, which is currently not supported!");
        return Task.FromResult(Enumerable.Empty<PackageReference>());
    }
}
