using System.IO.Abstractions;
using System.Xml.Linq;
using Liz.Core.License.Contracts;
using Liz.Core.License.Contracts.Models;
using Liz.Core.Logging;
using Liz.Core.Logging.Contracts;

namespace Liz.Core.License;

internal sealed class GetLicenseInformationFromArtifact : IGetLicenseInformationFromArtifact
{
    private const string NugetSpecificationFileExtension = "nuspec";
    private readonly IFileSystem _fileSystem;
    private readonly IEnumerable<ILicenseInformationSource> _licenseInformationSources;
    private readonly ILogger _logger;

    public GetLicenseInformationFromArtifact(
        IFileSystem fileSystem,
        ILogger logger,
        IEnumerable<ILicenseInformationSource> enrichLicenseInformationResults)
    {
        _fileSystem = fileSystem ?? throw new ArgumentNullException(nameof(fileSystem));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _licenseInformationSources = enrichLicenseInformationResults
                                     ?? throw new ArgumentNullException(nameof(enrichLicenseInformationResults));
    }

    public async Task<LicenseInformation> GetFromDownloadedPackageReferenceAsync(IDirectoryInfo downloadDirectory)
    {
        if (downloadDirectory == null)
        {
            throw new ArgumentNullException(nameof(downloadDirectory));
        }

        var licenseInformation = await GetFromArtifactAsync(downloadDirectory).ConfigureAwait(false);
        return licenseInformation;
    }

    private async Task<LicenseInformation> GetFromArtifactAsync(IDirectoryInfo artifactDirectory)
    {
        var licenseInformationContext = await CreateContext(artifactDirectory).ConfigureAwait(false);

        foreach (var licenseInformationSource in _licenseInformationSources.OrderBy(e => e.Order))
        {
            await licenseInformationSource.GetInformationAsync(licenseInformationContext).ConfigureAwait(false);
        }

        return licenseInformationContext.LicenseInformation;
    }

    private async Task<GetLicenseInformationContext> CreateContext(IDirectoryInfo artifactDirectory)
    {
        var licenseInformationContext = new GetLicenseInformationContext
        {
            ArtifactDirectory = artifactDirectory,
            NugetSpecificationFileXml = await GetNugetSpecificationFileXmlAsync(artifactDirectory).ConfigureAwait(false)
        };
        return licenseInformationContext;
    }

    private async Task<XDocument?> GetNugetSpecificationFileXmlAsync(IDirectoryInfo artifactDirectory)
    {
        if (!TryGetNugetSpecificationFile(artifactDirectory, out var nugetSpecificationFile) || nugetSpecificationFile == null)
        {
            _logger.LogWarning("No nuspec file was found in downloaded package reference!");
            return null; // return null is okay here
        }

        _logger.LogDebug($"Found '.nuspec' file: {nugetSpecificationFile}");

        var nugetSpecificationFileAsync =
            await LoadNugetSpecificationFileAsXmlFileAsync(nugetSpecificationFile).ConfigureAwait(false);
        return nugetSpecificationFileAsync;
    }

    private static bool TryGetNugetSpecificationFile(
        IDirectoryInfo artifactDirectory,
        out IFileInfo? nugetSpecificationFile)
    {
        nugetSpecificationFile = null;

        try
        {
            var candidates = artifactDirectory
                .EnumerateFiles(
                    $"*.{NugetSpecificationFileExtension}",
                    new EnumerationOptions
                    {
                        MatchCasing = MatchCasing.CaseInsensitive,
                        RecurseSubdirectories = false,
                        IgnoreInaccessible = true,
                        MatchType = MatchType.Simple
                    });

            var firstNuspec = candidates.FirstOrDefault();
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
        var xmlDocument = await XDocument
            .LoadAsync(fileStream, LoadOptions.None, CancellationToken.None)
            .ConfigureAwait(false);

        return xmlDocument;
    }
}
