using Liz.Core.Logging;
using Liz.Core.Logging.Contracts;
using Liz.Core.PackageReferences.Contracts.Models;
using Liz.Core.Result.Contracts;
using Liz.Core.Result.Contracts.Exceptions;
using Liz.Core.Settings;

namespace Liz.Core.Result;

internal sealed class ValidateLicenseTypesWhitelistResultProcessor : IResultProcessor
{
    private readonly ExtractLicensesSettingsBase _settings;
    private readonly ILogger _logger;

    public ValidateLicenseTypesWhitelistResultProcessor(
        ExtractLicensesSettingsBase settings,
        ILogger logger)
    {
        _settings = settings ?? throw new ArgumentNullException(nameof(settings));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public Task ProcessResultsAsync(IEnumerable<PackageReference> packageReferences)
    {
        if (packageReferences == null) throw new ArgumentNullException(nameof(packageReferences));

        var whitelist = _settings.LicenseTypeWhitelist;
        if (!whitelist.Any()) return Task.CompletedTask;
        
        _logger.LogDebug($"Validating package-references against whitelist ({string.Join(", ", whitelist)})");

        var invalidPackageReferences = DetermineInvalidPackageReferences(packageReferences, whitelist);
        if (!invalidPackageReferences.Any()) return Task.CompletedTask;

        ThrowInvalidException(invalidPackageReferences, whitelist);
        return Task.CompletedTask;
    }

    private static PackageReference[] DetermineInvalidPackageReferences(
        IEnumerable<PackageReference> packageReferences,
        IEnumerable<string> whitelist)
    {
        // violating a whitelist means, that there is a license-type that is not explicitly mentioned in the whitelist
        var packagesThatViolateWhitelist = packageReferences
            .Where(package => package.LicenseInformation.Types.Any())
            .Where(package => package
                .LicenseInformation
                .Types
                .All(type => !whitelist
                    .Any(whitelistEntry => whitelistEntry.Equals(type, StringComparison.InvariantCultureIgnoreCase))))
            .ToArray();

        return packagesThatViolateWhitelist;
    }
    
    private static void ThrowInvalidException(
        IEnumerable<PackageReference> invalidPackageReferences, 
        IEnumerable<string> whitelist)
    {
        var invalidPackagesDisplayMessages = invalidPackageReferences
            .Select(package => 
                $"> {package.Name} ({package.Version}): {string.Join(", ", package.LicenseInformation.Types)}");

        var message = "The determined package-references contain invalid license-types according to the provided " +
                      $"whitelist ({string.Join(", ", whitelist)}):{Environment.NewLine}{string.Join(Environment.NewLine, invalidPackagesDisplayMessages)}";

        throw new LicenseTypesInvalidException(message);
    }
}
