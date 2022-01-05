using JetBrains.Annotations;
using Liz.Core.Logging;
using System;
using System.IO.Abstractions;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Liz.Core.License.Sources;

internal sealed class LicenseElementLicenseInformationSource : ILicenseInformationSource
{
    private readonly IFileSystem _fileSystem;
    private readonly ILogger _logger;

    public LicenseElementLicenseInformationSource(
        [NotNull] ILogger logger,
        [NotNull] IFileSystem fileSystem)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _fileSystem = fileSystem ?? throw new ArgumentNullException(nameof(fileSystem));
    }

    // best to be at the start
    public int Order => 0;

    public async Task GetInformationAsync(GetLicenseInformationContext licenseInformationContext)
    {
        ArgumentNullException.ThrowIfNull(licenseInformationContext);

        if (licenseInformationContext.NugetSpecificationFileXml == null) return;

        _logger.LogDebug("Get license-information from 'license' element from the 'nuspec' file...");

        await GetLicenseInformationFromLicenseElementAsync(
            licenseInformationContext,
            licenseInformationContext.NugetSpecificationFileXml);
    }

    private async Task GetLicenseInformationFromLicenseElementAsync(
        GetLicenseInformationContext licenseInformationContext,
        XContainer nugetSpecificationFileXml)
    {
        if (!TryGetLicenseElement(nugetSpecificationFileXml, out var licenseElement)) return;

        _logger.LogDebug("Found 'license' element");
        await GetLicenseInformationBasedOnLicenseElementAsync(licenseInformationContext, licenseElement);
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
        GetLicenseInformationContext licenseInformationContext,
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
                HandleLicenseExpression(licenseInformationContext, licenseElementValue);
                break;
            case "file":
                await HandleLicenseFileAsync(licenseInformationContext, licenseElementValue);
                break;
            default:
                throw new InvalidOperationException(
                    $"Attribute 'type' value '{licenseElementTypeAttributeValue}' on element 'license' was not expected");
        }
    }

    private static void HandleLicenseExpression(
        GetLicenseInformationContext licenseInformationContext,
        string licenseElementValue)
    {
        licenseInformationContext.LicenseInformation.Type = licenseElementValue;
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
}
