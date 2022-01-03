using JetBrains.Annotations;
using Liz.Core.Logging;
using System;
using System.IO.Abstractions;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Liz.Core.License.Sources;

internal sealed class EnrichLicenseInformationFromLicenseElement : IEnrichLicenseInformationResult
{
    private readonly ILogger _logger;
    private readonly IFileSystem _fileSystem;

    public EnrichLicenseInformationFromLicenseElement(
        [NotNull] ILogger logger,
        [NotNull] IFileSystem fileSystem)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _fileSystem = fileSystem ?? throw new ArgumentNullException(nameof(fileSystem));
    }
    
    // best to be at the start
    public int Order => 0;
    
    public async Task EnrichAsync(GetLicenseInformationResult licenseInformationResult)
    {
        ArgumentNullException.ThrowIfNull(licenseInformationResult);

        if (licenseInformationResult.NugetSpecifiactionFileXml == null) return;
        
        _logger.LogDebug("Get license-information from 'license' element from the 'nuspec' file...");

        await GetLicenseInformationFromLicenseElementAsync(licenseInformationResult,
            licenseInformationResult.NugetSpecifiactionFileXml);
    }
    
    private async Task GetLicenseInformationFromLicenseElementAsync(
        GetLicenseInformationResult licenseInformationResult,
        XContainer nugetSpecificationFileXml)
    {
        if (!TryGetLicenseElement(nugetSpecificationFileXml, out var licenseElement)) return;

        _logger.LogDebug("Found 'license' element");
        await GetLicenseInformationBasedOnLicenseElementAsync(licenseInformationResult, licenseElement);
    }

    private bool TryGetLicenseElement(XContainer nugetSpecificationXml, out XElement licenseElement)
    {
        licenseElement = null;
        
        try
        {
            var packageElement = GetXmlElement(nugetSpecificationXml, "package");
            var metadataElement = GetXmlElement(packageElement, "metadata");
            licenseElement = GetXmlElement(metadataElement, "license");
        
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogDebug("Could not get 'license' element", ex);
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
    
    private async Task GetLicenseInformationBasedOnLicenseElementAsync(
        GetLicenseInformationResult licenseInformationResult,
        XElement licenseElement)
    {
        _logger.LogDebug("Getting license information from 'license' element...");
        
        var licenseElementTypeAttribute = licenseElement.Attribute("type");
        if (licenseElementTypeAttribute == null)
        {
            _logger.LogDebug("Could not find 'type' attribute on 'license' element. Aborting!");
            return;
        }

        var licenseElementTypeAttributeValue = licenseElementTypeAttribute.Value;
        var licenseElementValue = licenseElement.Value;
        
        _logger.LogDebug($"Attribute 'type' on element 'license' has value '{licenseElementTypeAttributeValue}' " +
                         $"and license element has value '{licenseElementValue}'");
        
        switch (licenseElementTypeAttributeValue)
        {
            case "expression":
                HandleLicenseExpression(licenseInformationResult, licenseElementValue);
                break;
            case "file":
                await HandleLicenseFileAsync(licenseInformationResult, licenseElementValue);
                break;
            default:
                throw new InvalidOperationException(
                    $"Attribute 'type' value '{licenseElementTypeAttributeValue}' on element 'license' was not expected");
        }
    }

    private static void HandleLicenseExpression(
        GetLicenseInformationResult licenseInformationResult,
        string licenseElementValue)
    {
        if (!string.IsNullOrWhiteSpace(licenseInformationResult.LicenseType)) return;
        
        licenseInformationResult.LicenseType = licenseElementValue;
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
}
