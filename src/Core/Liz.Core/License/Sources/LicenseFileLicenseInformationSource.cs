using Liz.Core.Logging;
using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using System.Threading.Tasks;

namespace Liz.Core.License.Sources;

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

    public LicenseFileLicenseInformationSource(
        [JetBrains.Annotations.NotNull] ILogger logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public int Order => 1;

    public async Task GetInformationAsync(GetLicenseInformationContext licenseInformationContext)
    {
        ArgumentNullException.ThrowIfNull(licenseInformationContext);

        if (licenseInformationContext.ArtifactDirectory == null) return;
        // we do not need to do this when there's already text
        if (!string.IsNullOrWhiteSpace(licenseInformationContext.LicenseInformation.Text)) return;

        _logger.LogDebug("Get license-information from file containing 'license'");

        await GetLicenseInformationFromLicenseFileAsync(
            licenseInformationContext.ArtifactDirectory,
            licenseInformationContext);
    }

    private async Task GetLicenseInformationFromLicenseFileAsync(
        IDirectoryInfo artifactDirectory,
        GetLicenseInformationContext licenseInformationContext)
    {
        if (!TryGetLicenseFile(artifactDirectory, out var licenseFile))
        {
            _logger.LogDebug("Could not find a possible license-file. Aborting!");
            return;
        }

        _logger.LogDebug($"Found possible license-file: {licenseFile}");
        _logger.LogDebug("Reading content as raw license-text...");

        using var streamReader = licenseFile.OpenText();
        var licenseText = await streamReader.ReadToEndAsync();
        licenseInformationContext.LicenseInformation.Text = licenseText;
    }

    private static bool TryGetLicenseFile(
        IDirectoryInfo artifactDirectory,
        out IFileInfo licenseFile)
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
