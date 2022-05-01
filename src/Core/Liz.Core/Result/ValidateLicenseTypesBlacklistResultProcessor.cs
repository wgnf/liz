using Liz.Core.Logging;
using Liz.Core.Logging.Contracts;
using Liz.Core.PackageReferences.Contracts.Models;
using Liz.Core.Result.Contracts;
using Liz.Core.Result.Contracts.Exceptions;
using Liz.Core.Settings;

namespace Liz.Core.Result;

internal sealed class ValidateLicenseTypesBlacklistResultProcessor : IResultProcessor
{
    private readonly ExtractLicensesSettingsBase _settings;
    private readonly ILogger _logger;

    public ValidateLicenseTypesBlacklistResultProcessor(
        ExtractLicensesSettingsBase settings, 
        ILogger logger)
    {
        _settings = settings ?? throw new ArgumentNullException(nameof(settings));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public Task ProcessResultsAsync(IEnumerable<PackageReference> packageReferences)
    {
        if (packageReferences == null) throw new ArgumentNullException(nameof(packageReferences));

        var blacklist = _settings.LicenseTypeBlacklist;
        if (!blacklist.Any()) return Task.CompletedTask;

        _logger.LogDebug($"Validating package-references against blacklist ({string.Join(", ", blacklist)})");

        var invalidPackageReferences = DetermineInvalidPackageReferences(packageReferences, blacklist);
        if (!invalidPackageReferences.Any()) return Task.CompletedTask;

        ThrowInvalidException(invalidPackageReferences, blacklist);
        return Task.CompletedTask;
    }

    private static PackageReference[] DetermineInvalidPackageReferences(
        IEnumerable<PackageReference> packageReferences, 
        IEnumerable<string> blacklist)
    {
        // violating a blacklist means, that there is a license-type that is explicitly mentioned in the blacklist
        var packagesThatViolateBlacklist = packageReferences
            .Where(package => package
                .LicenseInformation
                .Types
                .Any(type => blacklist
                    .Any(blacklistEntry => blacklistEntry.Equals(type, StringComparison.InvariantCultureIgnoreCase))))
            .ToArray();

        return packagesThatViolateBlacklist;
    }

    private static void ThrowInvalidException(
        IEnumerable<PackageReference> invalidPackageReferences, 
        IEnumerable<string> blacklist)
    {
        var invalidPackagesDisplayMessages = invalidPackageReferences
            .Select(package =>
                $"> {package.Name} ({package.Version}): {string.Join(", ", package.LicenseInformation.Types)}");

        var message = "The determined package-references contain invalid license-types according to the provided" +
                      $"blacklist ({string.Join(", ", blacklist)}):\n{string.Join("\n", invalidPackagesDisplayMessages)}";

        throw new LicenseTypesInvalidException(message);
    }
}
