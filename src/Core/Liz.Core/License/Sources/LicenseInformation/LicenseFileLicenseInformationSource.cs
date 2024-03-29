﻿using System.Diagnostics.CodeAnalysis;
using System.IO.Abstractions;
using Liz.Core.License.Contracts;
using Liz.Core.License.Contracts.Models;
using Liz.Core.Logging;
using Liz.Core.Logging.Contracts;

namespace Liz.Core.License.Sources.LicenseInformation;

/*
 * NOTE / TODO:
 * Unfortunately testing this is a huge pain in the ass right now, because this depends on
 * "IDirectoryInfo.EnumerateFiles" with the option "MatchingCase.CaseInsensitive" AND the unit testing helpers
 * of System.IO.Abstractions don't support "MatchingCase.CaseInsensitive" for their "MockDirectoryInfo" yet...
 *
 * So this will be tested once this works...
 */

[ExcludeFromCodeCoverage]
internal sealed class LicenseFileLicenseInformationSource : ILicenseInformationSource
{
    private readonly ILogger _logger;

    public LicenseFileLicenseInformationSource(ILogger logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public int Order => 1;

    public async Task GetInformationAsync(GetLicenseInformationContext licenseInformationContext)
    {
        if (licenseInformationContext == null)
        {
            throw new ArgumentNullException(nameof(licenseInformationContext));
        }

        if (licenseInformationContext.ArtifactDirectory == null)
        {
            return;
        }

        // we do not need to do this when there's already text
        if (!string.IsNullOrWhiteSpace(licenseInformationContext.LicenseInformation.Text))
        {
            return;
        }

        _logger.LogDebug("Get license-information from a file containing 'license'");

        await GetLicenseInformationFromLicenseFileAsync(
            licenseInformationContext.ArtifactDirectory,
            licenseInformationContext).ConfigureAwait(false);
    }

    private async Task GetLicenseInformationFromLicenseFileAsync(
        IDirectoryInfo artifactDirectory,
        GetLicenseInformationContext licenseInformationContext)
    {
        if (!TryGetLicenseFile(artifactDirectory, out var licenseFile) || licenseFile == null)
        {
            _logger.LogDebug("Could not find a possible license-file. Aborting!");
            return;
        }

        _logger.LogDebug($"Found possible license-file: {licenseFile}");
        _logger.LogDebug("Reading content as raw license-text...");

        using var streamReader = licenseFile.OpenText();
        var licenseText = await streamReader.ReadToEndAsync().ConfigureAwait(false);
        licenseInformationContext.LicenseInformation.Text = licenseText;
    }

    private static bool TryGetLicenseFile(
        IDirectoryInfo artifactDirectory,
        out IFileInfo? licenseFile)
    {
        licenseFile = null;

        try
        {
            var candidates = artifactDirectory
                .EnumerateFiles(
                    "*license*",
                    new EnumerationOptions
                    {
                        MatchCasing = MatchCasing.CaseInsensitive,
                        RecurseSubdirectories = false,
                        IgnoreInaccessible = true,
                        MatchType = MatchType.Simple
                    });

            licenseFile = candidates.FirstOrDefault();
            return licenseFile is { Exists: true };
        }
        catch (Exception)
        {
            return false;
        }
    }
}
