using JetBrains.Annotations;
using Liz.Core.Logging;
using System;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using System.Threading.Tasks;

namespace Liz.Core.License.Sources;

internal sealed class LicenseFileLicenseInformationSource : ILicenseInformationSource
{
    private readonly ILogger _logger;

    public LicenseFileLicenseInformationSource(
        [NotNull] ILogger logger)
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
