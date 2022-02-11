using Liz.Core.License.Contracts;
using Liz.Core.License.Contracts.Models;
using Liz.Core.Logging;
using Liz.Core.Logging.Contracts;
using Liz.Core.Utils.Contracts.Wrappers;
using System.IO.Abstractions;
using System.Xml.Linq;

namespace Liz.Core.License.Sources;

internal sealed class LicenseUrlElementLicenseInformationSource : ILicenseInformationSource
{
    private readonly ILogger _logger;
    private readonly IFileSystem _fileSystem;
    private readonly IHttpClient _httpClient;

    public LicenseUrlElementLicenseInformationSource(ILogger logger, IFileSystem fileSystem, IHttpClient httpClient)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _fileSystem = fileSystem ?? throw new ArgumentNullException(nameof(fileSystem));
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
    }
    
    public int Order => 2;
    
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
        if (!TryGetLicenseUrlElement(nugetSpecificationFileXml, out var licenseUrlElement) || licenseUrlElement == null) 
            return;
        
        _logger.LogDebug("Found 'licenseUrl' element");
        await GetLicenseInformationBasedOnLicenseUrlElementAsync(licenseInformationContext, licenseUrlElement);
    }

    private bool TryGetLicenseUrlElement(XContainer nugetSpecificationXml, out XElement? licenseUrlElement)
    {
        licenseUrlElement = null;
        
        try
        {
            /*
             * NOTE:
             * Unfortunately, because the "licenseUrl" element belongs to a namespace it should be addressed with the
             * namespace as the name. But this does not seem to work, so we'll have to use this approach
             */
            licenseUrlElement = nugetSpecificationXml
                .Descendants()
                .FirstOrDefault(element => element.Name.LocalName == "licenseUrl");

            if (licenseUrlElement != null) return true;
            
            _logger.LogDebug("Could not get 'license' element");
            return false;
        }
        catch (Exception ex)
        {
            _logger.LogDebug("Could not get 'licenseUrl' element", ex);
            return false;
        }
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
        var isWebResource = licenseUrl.StartsWith("http");
        if (isWebResource)
            await HandleLicenseWebResourceAsync(licenseInformationContext, licenseUrl);
        else
            await HandleLicenseFileAsync(licenseInformationContext, licenseUrl);
    }

    private async Task HandleLicenseFileAsync(
        GetLicenseInformationContext licenseInformationContext,
        string licenseElementValue)
    {
        if (licenseInformationContext.ArtifactDirectory == null)
            return;
        
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
