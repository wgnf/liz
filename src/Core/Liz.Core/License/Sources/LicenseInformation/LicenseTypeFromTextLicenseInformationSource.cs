﻿using Liz.Core.License.Contracts;
using Liz.Core.License.Contracts.Models;
using Liz.Core.Logging;
using Liz.Core.Logging.Contracts;

namespace Liz.Core.License.Sources.LicenseInformation;

internal sealed class LicenseTypeFromTextLicenseInformationSource : ILicenseInformationSource
{
    private readonly IEnumerable<ILicenseTypeDefinitionProvider> _licenseTypeDefinitionProviders;
    private readonly ILogger _logger;

    private bool _isInitialized;
    private IEnumerable<LicenseTypeDefinition> _typeDefinitions;

    public LicenseTypeFromTextLicenseInformationSource(
        IEnumerable<ILicenseTypeDefinitionProvider> licenseTypeDefinitionProviders,
        ILogger logger)
    {
        _licenseTypeDefinitionProviders = licenseTypeDefinitionProviders ??
                                          throw new ArgumentNullException(nameof(licenseTypeDefinitionProviders));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        _typeDefinitions = Enumerable.Empty<LicenseTypeDefinition>();
    }

    // needs to be after the stuff that possibly gets the license-text
    public int Order => 10;

    public Task GetInformationAsync(GetLicenseInformationContext licenseInformationContext)
    {
        if (licenseInformationContext == null)
        {
            throw new ArgumentNullException(nameof(licenseInformationContext));
        }

        Initialize();

        // no need to attempt getting the license-type from the text, when there's no text, duh
        if (string.IsNullOrWhiteSpace(licenseInformationContext.LicenseInformation.Text))
        {
            return Task.CompletedTask;
        }

        var licenseText = RemoveControlCharacters(licenseInformationContext.LicenseInformation.Text);

        /*
         * NOTE:
         * Ignoring Webpages for now (until sanitation (https://github.com/wgnf/liz/issues/28) works reliably), because
         * some webpages may target a specific license-type (i.e. https://choosealicense.com/licenses/mit/) but also
         * have phrases in links and scripts that target other license types
         */
        if (licenseText.Contains("<!DOCTYPE html>", StringComparison.InvariantCultureIgnoreCase))
        {
            return Task.CompletedTask;
        }

        _logger.LogDebug(
            $"Attempting to get license-type from license-text for\n{licenseInformationContext.LicenseInformation.Text}");

        HandleTypeDefinitions(licenseInformationContext, licenseText);

        return Task.CompletedTask;
    }

    private void HandleTypeDefinitions(GetLicenseInformationContext licenseInformationContext, string licenseText)
    {
        foreach (var typeDefinition in _typeDefinitions)
        {
            HandleTypeDefinition(licenseInformationContext, licenseText, typeDefinition);
        }
    }

    private static void HandleTypeDefinition(
        GetLicenseInformationContext licenseInformationContext,
        string licenseText,
        LicenseTypeDefinition typeDefinition)
    {
        // no need to check if it already is contained
        if (licenseInformationContext.LicenseInformation.Types.Contains(typeDefinition.LicenseType))
        {
            return;
        }

        var containsAllSnippets = typeDefinition
            .InclusiveTextSnippets
            .All(snippet => licenseText.Contains(RemoveControlCharacters(snippet), StringComparison.InvariantCultureIgnoreCase));

        var containsAnExclusion = typeDefinition
            .ExclusiveTextSnippets
            .Any(snippet => licenseText.Contains(RemoveControlCharacters(snippet), StringComparison.InvariantCultureIgnoreCase));

        if (containsAllSnippets && !containsAnExclusion)
        {
            licenseInformationContext.LicenseInformation.AddLicenseType(typeDefinition.LicenseType);
        }
    }

    private static string RemoveControlCharacters(string input)
    {
        return new string(input.Where(character => !char.IsControl(character)).ToArray());
    }

    /*
     * NOTE:
     * We need this here, because some definitions come from the settings which are partly set by a preprocessor,
     * which has not been executed yet, when the constructor is called
     */
    private void Initialize()
    {
        if (_isInitialized)
        {
            return;
        }

        _typeDefinitions = _licenseTypeDefinitionProviders.SelectMany(provider => provider.Get());
        _isInitialized = true;
    }
}
