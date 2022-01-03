using JetBrains.Annotations;
using Liz.Core.License.Sources;
using Liz.Core.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Liz.Core.License;

internal sealed class GetLicenseInformationFromArtifact : IGetLicenseInformationFromArtifact
{
    private readonly IFileSystem _fileSystem;
    private readonly ILogger _logger;
    private readonly IEnumerable<IEnrichLicenseInformationResult> _enrichLicenseInformationResults;
    
    private const string NugetSpecificationFileExtension = "nuspec";

    public GetLicenseInformationFromArtifact(
        [NotNull] IFileSystem fileSystem,
        [NotNull] ILogger logger,
        [NotNull] IEnumerable<IEnrichLicenseInformationResult> enrichLicenseInformationResults)
    {
        _fileSystem = fileSystem ?? throw new ArgumentNullException(nameof(fileSystem));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _enrichLicenseInformationResults = enrichLicenseInformationResults 
                                           ?? throw new ArgumentNullException(nameof(enrichLicenseInformationResults));
    }
    
    public async Task<GetLicenseInformationResult> GetFromDownloadedPackageReferenceAsync(IDirectoryInfo downloadDirectory)
    {
        ArgumentNullException.ThrowIfNull(downloadDirectory);

        var licenseInformationResult = await GetFromArtifactAsync(downloadDirectory);
        return licenseInformationResult;
    }

    private async Task<GetLicenseInformationResult> GetFromArtifactAsync(IDirectoryInfo artifactDirectory)
    {
        var licenseInformationResult = new GetLicenseInformationResult
        {
            ArtifactDirectory = artifactDirectory,
            NugetSpecifiactionFileXml = await GetNugetSpecificationFileXmlAsync(artifactDirectory)
        };

        foreach (var enrichLicenseInformationResult in _enrichLicenseInformationResults.OrderBy(e => e.Order))
            await enrichLicenseInformationResult.EnrichAsync(licenseInformationResult);

        return licenseInformationResult;
    }

    private async Task<XDocument> GetNugetSpecificationFileXmlAsync(IDirectoryInfo artifactDirectory)
    {
        if (!TryGetNugetSpecificationFile(artifactDirectory, out var nugetSpecificationFile))
            return null; // return null is okay here
        
        _logger.LogDebug($"Found '.nuspec' file: {nugetSpecificationFile}");

        var nugetSpecificationFileAsync = await LoadNugetSpecificationFileAsXmlFileAsync(nugetSpecificationFile);
        return nugetSpecificationFileAsync;
    }
    
    private static bool TryGetNugetSpecificationFile(IDirectoryInfo artifactDirectory, out IFileInfo nugetSpecificationFile)
    {
        nugetSpecificationFile = null;
        
        try
        {
            var files = artifactDirectory.GetFiles(
                $"*.{NugetSpecificationFileExtension}", 
                SearchOption.TopDirectoryOnly);

            var firstNuspec = files.FirstOrDefault();
            nugetSpecificationFile = firstNuspec;
            return nugetSpecificationFile is { Exists: true };
        }
        catch (Exception)
        {
            return false;
        }
    }

    private async Task<XDocument> LoadNugetSpecificationFileAsXmlFileAsync(IFileSystemInfo nugetSpecificationFile)
    {
        await using var fileStream =
            _fileSystem.FileStream.Create(nugetSpecificationFile.FullName, FileMode.Open, FileAccess.Read);
        var xmlDocument = await XDocument.LoadAsync(fileStream, LoadOptions.None, CancellationToken.None);

        return xmlDocument;
    }
}
