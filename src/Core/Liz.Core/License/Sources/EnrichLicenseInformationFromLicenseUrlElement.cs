using JetBrains.Annotations;
using Liz.Core.Logging;
using System;
using System.IO.Abstractions;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Liz.Core.License.Sources;

internal sealed class EnrichLicenseInformationFromLicenseUrlElement : IEnrichLicenseInformationResult
{
    private readonly ILogger _logger;
    private readonly IFileSystem _fileSystem;

    public EnrichLicenseInformationFromLicenseUrlElement(
        [NotNull] ILogger logger,
        [NotNull] IFileSystem fileSystem)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _fileSystem = fileSystem ?? throw new ArgumentNullException(nameof(fileSystem));
    }
    
    public int Order => 1;
    
    public async Task EnrichAsync(GetLicenseInformationResult licenseInformationResult)
    {
        ArgumentNullException.ThrowIfNull(licenseInformationResult);

        if (licenseInformationResult.NugetSpecifiactionFileXml == null) return;
        
        _logger.LogDebug("Get license-information from 'licenseUrl' element from the 'nuspec' file...");

        await GetLicenseInformationFromLicenseUrlElementAsync(licenseInformationResult, licenseInformationResult.NugetSpecifiactionFileXml);
    }

    private async Task GetLicenseInformationFromLicenseUrlElementAsync
        (GetLicenseInformationResult licenseInformationResult, 
            XContainer nugetSpecificationFileXml)
    {
        if (!TryGetLicenseUrlElement(nugetSpecificationFileXml, out var licenseUrlElement)) return;
        
        _logger.LogDebug("Found 'licenseUrl' element");
        await GetLicenseInformationBasedOnLicenseUrlElementAsync(licenseInformationResult, licenseUrlElement);
    }

    private bool TryGetLicenseUrlElement(XContainer nugetSpecificationXml, out XElement licenseUrlElement)
    {
        licenseUrlElement = null;
        
        try
        {
            var packageElement = GetXmlElement(nugetSpecificationXml, "package");
            var metadataElement = GetXmlElement(packageElement, "metadata");
            licenseUrlElement = GetXmlElement(metadataElement, "licenseUrl");
        
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogDebug("Could not get 'licenseUrl' element", ex);
            return false;
        }
    }

    private static XElement GetXmlElement(XContainer xmlContainer, string elementName)
    {
        /*
         * NOTE:
         * This has to be done this way, because the actual name (which you also use in 'Element(name)' or
         * 'Elements(name)' or 'XPathSelectElement(name)' actually look this way: '{some.xml.namespace}name' but i do
         * not really want to use the XML-Namespace everywhere...
         */
        var packageElement = xmlContainer
            .Elements()
            .FirstOrDefault(element => element.Name.LocalName == elementName);
        if (packageElement == null)
            throw new InvalidOperationException($"Could not find a '{elementName}' element");
        
        return packageElement;
    }

    private async Task GetLicenseInformationBasedOnLicenseUrlElementAsync(
        GetLicenseInformationResult licenseInformationResult, 
        XElement licenseUrlElement)
    {
        _logger.LogDebug("Getting license information from 'licenseUrl' element...");

        if (!string.IsNullOrWhiteSpace(licenseInformationResult.RawLicenseText)) return;

        var licenseUrlElementValue = licenseUrlElement.Value;
        await HandleLicenseUrlAsync(licenseInformationResult, licenseUrlElementValue);
    }

    private async Task HandleLicenseUrlAsync(GetLicenseInformationResult licenseInformationResult, string licenseUrl)
    {
        if (!string.IsNullOrWhiteSpace(licenseInformationResult.LicenseUrl))
            licenseInformationResult.LicenseUrl = licenseUrl;
        
        var licenseUri = new Uri(licenseUrl, UriKind.RelativeOrAbsolute);
        if (licenseUri.IsFile)
            await HandleLicenseFileAsync(licenseInformationResult, licenseUrl);
        else
            await HandleLicenseWebResourceAsync(licenseInformationResult, licenseUrl);
    }

    private async Task HandleLicenseFileAsync(
        GetLicenseInformationResult licenseInformationResult,
        string licenseElementValue)
    {
        if (!string.IsNullOrWhiteSpace(licenseInformationResult.RawLicenseText)) return;
        
        var licenseFile =
            _fileSystem.Path.Combine(licenseInformationResult.ArtifactDirectory.FullName, licenseElementValue);
        var licenseFileInfo = _fileSystem.FileInfo.FromFileName(licenseFile);
        
        _logger.LogDebug($"Specified license file should be: '{licenseFileInfo}'");

        if (!licenseFileInfo.Exists)
        {
            _logger.LogDebug("Specified license file could not be found");
            return;
        }

        var rawLicenseTextFromFile = await _fileSystem.File.ReadAllTextAsync(licenseFileInfo.FullName);
        licenseInformationResult.RawLicenseText = rawLicenseTextFromFile;
    }
    
    private async Task HandleLicenseWebResourceAsync(GetLicenseInformationResult licenseInformationResult, string licenseUrl)
    {
        if (!string.IsNullOrWhiteSpace(licenseInformationResult.RawLicenseText)) return;

        var httpClient = new HttpClient();
        _logger.LogDebug($"Downloading raw license-text from '{licenseUrl}'...");

        try
        {
            var rawLicenseText = await httpClient.GetStringAsync(licenseUrl);
            licenseInformationResult.RawLicenseText = rawLicenseText;
        }
        catch (Exception ex)
        {
            _logger.LogWarning($"Unable to download raw license-text from '{licenseUrl}'", ex);
        }
    }
}
