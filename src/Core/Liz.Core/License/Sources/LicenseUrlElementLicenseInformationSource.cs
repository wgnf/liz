using JetBrains.Annotations;
using Liz.Core.Logging;
using Liz.Core.Utils.Wrappers;
using System;
using System.IO.Abstractions;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Liz.Core.License.Sources;

internal sealed class LicenseUrlElementLicenseInformationSource : ILicenseInformationSource
{
    private readonly ILogger _logger;
    private readonly IFileSystem _fileSystem;
    private readonly IHttpClient _httpClient;

    public LicenseUrlElementLicenseInformationSource(
        [NotNull] ILogger logger,
        [NotNull] IFileSystem fileSystem,
        [NotNull] IHttpClient httpClient)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _fileSystem = fileSystem ?? throw new ArgumentNullException(nameof(fileSystem));
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
    }
    
    public int Order => 1;
    
    public async Task GetInformationAsync(GetLicenseInformationContext licenseInformationContext)
    {
        ArgumentNullException.ThrowIfNull(licenseInformationContext);

        if (licenseInformationContext.NugetSpecificationFileXml == null) return;
        
        _logger.LogDebug("Get license-information from 'licenseUrl' element from the 'nuspec' file...");

        await GetLicenseInformationFromLicenseUrlElementAsync(
            licenseInformationContext, 
            licenseInformationContext.NugetSpecificationFileXml);
    }

    private async Task GetLicenseInformationFromLicenseUrlElementAsync(
        GetLicenseInformationContext licenseInformationContext, 
        XContainer nugetSpecificationFileXml)
    {
        if (!TryGetLicenseUrlElement(nugetSpecificationFileXml, out var licenseUrlElement)) return;
        
        _logger.LogDebug("Found 'licenseUrl' element");
        await GetLicenseInformationBasedOnLicenseUrlElementAsync(licenseInformationContext, licenseUrlElement);
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
        GetLicenseInformationContext licenseInformationContext, 
        XElement licenseUrlElement)
    {
        _logger.LogDebug("Getting license information from 'licenseUrl' element...");

        var licenseUrlElementValue = licenseUrlElement.Value;
        licenseInformationContext.LicenseInformation.Url = licenseUrlElementValue;
        
        /*
         * NOTE:
         * We abort early here, to save time and resources when the license-text was already extracted.
         * Because when it already was determined we can assume that it's already the right one
         */
        if (!string.IsNullOrWhiteSpace(licenseInformationContext.LicenseInformation.Text)) return;
        await HandleLicenseUrlAsync(licenseInformationContext, licenseUrlElementValue);
    }

    private async Task HandleLicenseUrlAsync(GetLicenseInformationContext licenseInformationContext, string licenseUrl)
    {
        var licenseUri = new Uri(licenseUrl, UriKind.RelativeOrAbsolute);
        if (licenseUri.IsFile)
            await HandleLicenseFileAsync(licenseInformationContext, licenseUrl);
        else
            await HandleLicenseWebResourceAsync(licenseInformationContext, licenseUrl);
    }

    private async Task HandleLicenseFileAsync(
        GetLicenseInformationContext licenseInformationContext,
        string licenseElementValue)
    {
        var licenseFile =
            _fileSystem.Path.Combine(licenseInformationContext.ArtifactDirectory.FullName, licenseElementValue);
        var licenseFileInfo = _fileSystem.FileInfo.FromFileName(licenseFile);
        
        _logger.LogDebug($"Specified license file should be: '{licenseFileInfo}'");

        if (!licenseFileInfo.Exists)
        {
            _logger.LogDebug("Specified license file could not be found");
            return;
        }

        var rawLicenseTextFromFile = await _fileSystem.File.ReadAllTextAsync(licenseFileInfo.FullName);
        licenseInformationContext.LicenseInformation.Text = rawLicenseTextFromFile;
    }
    
    private async Task HandleLicenseWebResourceAsync(
        GetLicenseInformationContext licenseInformationContext, 
        string licenseUrl)
    {
        _logger.LogDebug($"Downloading raw license-text from '{licenseUrl}'...");

        try
        {
            var rawLicenseText = await _httpClient.GetStringAsync(licenseUrl);
            licenseInformationContext.LicenseInformation.Text = rawLicenseText;
        }
        catch (Exception ex)
        {
            _logger.LogWarning($"Unable to download raw license-text from '{licenseUrl}'", ex);
        }
    }
}
