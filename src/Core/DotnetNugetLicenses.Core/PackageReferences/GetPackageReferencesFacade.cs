using DotnetNugetLicenses.Core.Logging;
using DotnetNugetLicenses.Core.PackageReferences.DotnetCli;
using DotnetNugetLicenses.Core.Projects;
using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace DotnetNugetLicenses.Core.PackageReferences;

internal sealed class GetPackageReferencesFacade : IGetPackageReferences
{
    private readonly IFileSystem _fileSystem;
    private readonly IGetPackageReferencesViaDotnetCli _getPackageReferencesViaDotnetCli;
    private readonly ILogger _logger;

    public GetPackageReferencesFacade(
        [NotNull] ILogger logger,
        [NotNull] IFileSystem fileSystem,
        [NotNull] IGetPackageReferencesViaDotnetCli getPackageReferencesViaDotnetCli)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _fileSystem = fileSystem ?? throw new ArgumentNullException(nameof(fileSystem));
        _getPackageReferencesViaDotnetCli = getPackageReferencesViaDotnetCli ??
                                            throw new ArgumentNullException(nameof(getPackageReferencesViaDotnetCli));
    }

    public async Task<IEnumerable<PackageReference>> GetFromProjectAsync(Project project, bool includeTransitive)
    {
        if (project == null) throw new ArgumentNullException(nameof(project));
        if (!project.File.Exists)
            throw new FileNotFoundException($"Project file '{project.File}' does not exist");

        if (await IsSdkStyleAsync(project.File))
        {
            _logger.LogDebug(
                $"Project '{project.Name} ({project.File})' appears to be in SDK-Style-Format. Getting package-references via dotnet cli...");
            return await _getPackageReferencesViaDotnetCli.GetFromProjectAsync(project, includeTransitive);
        }

        _logger.LogDebug(
            $"Project '{project.Name} ({project.File})' appears to be in non-SDK-style-Format. Getting package-references via packages.config...");

        _logger.LogWarning(
            $"Project '{project.Name} ({project.File})' has the non-SDK-style-Format, which is currently not supported!");
        return Enumerable.Empty<PackageReference>();
    }

    private async Task<bool> IsSdkStyleAsync(IFileSystemInfo projectFile)
    {
        var fileStream = _fileSystem.FileStream.Create(projectFile.FullName, FileMode.Open, FileAccess.Read);
        var xmlDocument = await XDocument.LoadAsync(fileStream, LoadOptions.None, CancellationToken.None);

        var projectRoot = xmlDocument
            .Elements("Project")
            .FirstOrDefault();

        var hasSdkAttribute = projectRoot?.Attribute("Sdk") != null;
        var hasSdkElement = xmlDocument.Element("Sdk") != null;

        return hasSdkAttribute || hasSdkElement;
    }
}
