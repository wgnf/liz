﻿using JetBrains.Annotations;
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
    private const string NugetSpecificationFileExtension = "nuspec";
    private readonly IEnumerable<ILicenseInformationSource> _enrichLicenseInformationResults;
    private readonly IFileSystem _fileSystem;
    private readonly ILogger _logger;

    public GetLicenseInformationFromArtifact(
        [NotNull] IFileSystem fileSystem,
        [NotNull] ILogger logger,
        [NotNull] IEnumerable<ILicenseInformationSource> enrichLicenseInformationResults)
    {
        _fileSystem = fileSystem ?? throw new ArgumentNullException(nameof(fileSystem));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _enrichLicenseInformationResults = enrichLicenseInformationResults
                                           ?? throw new ArgumentNullException(nameof(enrichLicenseInformationResults));
    }

    public async Task<LicenseInformation> GetFromDownloadedPackageReferenceAsync(IDirectoryInfo downloadDirectory)
    {
        ArgumentNullException.ThrowIfNull(downloadDirectory);

        var licenseInformation = await GetFromArtifactAsync(downloadDirectory);
        return licenseInformation;
    }

    private async Task<LicenseInformation> GetFromArtifactAsync(IDirectoryInfo artifactDirectory)
    {
        var licenseInformationContext = await GetContext(artifactDirectory);

        foreach (var enrichLicenseInformationResult in _enrichLicenseInformationResults.OrderBy(e => e.Order))
            await enrichLicenseInformationResult.GetInformationAsync(licenseInformationContext);

        return licenseInformationContext.LicenseInformation;
    }

    private async Task<GetLicenseInformationContext> GetContext(IDirectoryInfo artifactDirectory)
    {
        var licenseInformationContext = new GetLicenseInformationContext
        {
            ArtifactDirectory = artifactDirectory,
            NugetSpecificationFileXml = await GetNugetSpecificationFileXmlAsync(artifactDirectory)
        };
        return licenseInformationContext;
    }

    private async Task<XDocument> GetNugetSpecificationFileXmlAsync(IDirectoryInfo artifactDirectory)
    {
        if (!TryGetNugetSpecificationFile(artifactDirectory, out var nugetSpecificationFile))
            return null; // return null is okay here

        _logger.LogDebug($"Found '.nuspec' file: {nugetSpecificationFile}");

        var nugetSpecificationFileAsync = await LoadNugetSpecificationFileAsXmlFileAsync(nugetSpecificationFile);
        return nugetSpecificationFileAsync;
    }

    private static bool TryGetNugetSpecificationFile(IDirectoryInfo artifactDirectory,
        out IFileInfo nugetSpecificationFile)
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
