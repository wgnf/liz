﻿using System.IO.Abstractions;
using System.Xml.Linq;
using Liz.Core.License.Contracts;
using Liz.Core.License.Contracts.Models;
using Liz.Core.Logging;
using Liz.Core.Logging.Contracts;

namespace Liz.Core.License.Sources.LicenseInformation;

/*
 * NOTE:
 * There's no check here if LicenseInformation.Text or LicenseInformation.Type is set, because:
 * The values that are provided by the 'license' element of the .nuspec always are the most accurate,
 * because it either defines a license-type (type="expression") or a file containing the license-text
 * (type="file") which is both controlled by the author
 *
 * The other sources are just _extras_ to hopefully gather everything that can be gathered and are "best guesses"
 * and so should never override anything that was already specifically set by the author
 */
internal sealed class LicenseElementLicenseInformationSource : ILicenseInformationSource
{
    private readonly IFileSystem _fileSystem;
    private readonly ILogger _logger;

    public LicenseElementLicenseInformationSource(ILogger logger, IFileSystem fileSystem)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _fileSystem = fileSystem ?? throw new ArgumentNullException(nameof(fileSystem));
    }

    // best to be at the start
    public int Order => 0;

    public async Task GetInformationAsync(GetLicenseInformationContext licenseInformationContext)
    {
        if (licenseInformationContext == null)
        {
            throw new ArgumentNullException(nameof(licenseInformationContext));
        }

        if (licenseInformationContext.NugetSpecificationFileXml == null)
        {
            return;
        }

        _logger.LogDebug("Get license-information from 'license' element from the 'nuspec' file...");

        await GetLicenseInformationFromLicenseElementAsync(
            licenseInformationContext,
            licenseInformationContext.NugetSpecificationFileXml).ConfigureAwait(false);
    }

    private async Task GetLicenseInformationFromLicenseElementAsync(
        GetLicenseInformationContext licenseInformationContext,
        XContainer nugetSpecificationFileXml)
    {
        if (!TryGetLicenseElement(nugetSpecificationFileXml, out var licenseElement) || licenseElement == null)
        {
            return;
        }

        _logger.LogDebug("Found 'license' element");
        await GetLicenseInformationBasedOnLicenseElementAsync(licenseInformationContext, licenseElement)
            .ConfigureAwait(false);
    }

    private bool TryGetLicenseElement(XContainer nugetSpecificationXml, out XElement? licenseElement)
    {
        licenseElement = null;

        try
        {
            /*
             * NOTE:
             * Unfortunately, because the "license" element belongs to a namespace it should be addressed with the
             * namespace as the name. But this does not seem to work, so we'll have to use this approach
             */
            licenseElement = nugetSpecificationXml
                .Descendants()
                .FirstOrDefault(element => element.Name.LocalName == "license");

            if (licenseElement != null)
            {
                return true;
            }

            _logger.LogDebug("Could not get 'license' element");
            return false;
        }
        catch (Exception ex)
        {
            _logger.LogDebug("Could not get 'license' element", ex);
            return false;
        }
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
                await HandleLicenseFileAsync(licenseInformationContext, licenseElementValue).ConfigureAwait(false);
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
        licenseInformationContext.LicenseInformation.AddLicenseType(licenseElementValue);
    }

    private async Task HandleLicenseFileAsync(
        GetLicenseInformationContext licenseInformationContext,
        string licenseElementValue)
    {
        if (licenseInformationContext.ArtifactDirectory == null)
        {
            return;
        }

        var licenseFile = _fileSystem
            .Path
            .Combine(licenseInformationContext.ArtifactDirectory.FullName, licenseElementValue);

        var licenseFileInfo = _fileSystem
            .FileInfo
            .FromFileName(licenseFile);

        _logger.LogDebug($"Specified license file should be: '{licenseFileInfo}'");

        if (!licenseFileInfo.Exists)
        {
            _logger.LogDebug("Specified license file could not be found");
            return;
        }

        var rawLicenseTextFromFile =
            await _fileSystem.File.ReadAllTextAsync(licenseFileInfo.FullName).ConfigureAwait(false);
        licenseInformationContext.LicenseInformation.Text = rawLicenseTextFromFile;
    }
}
