using Liz.Core.License.Contracts;
using Liz.Core.License.Contracts.Models;
using Liz.Core.Logging;
using Liz.Core.Logging.Contracts;

namespace Liz.Core.License.Sources.LicenseInformation;

internal sealed class UrlToLicenseTypeMappingLicenseInformationSource : ILicenseInformationSource
{
    private readonly ILogger _logger;
    private readonly IEnumerable<IUrlToLicenseTypeMappingProvider> _urlToLicenseTypeMappingProviders;

    private bool _isInitialized;
    private Dictionary<string, string> _urlToLicenseTypeMappings;

    public UrlToLicenseTypeMappingLicenseInformationSource(
        IEnumerable<IUrlToLicenseTypeMappingProvider> urlToLicenseTypeMappingProviders,
        ILogger logger)
    {
        _urlToLicenseTypeMappingProviders = urlToLicenseTypeMappingProviders ??
                                            throw new ArgumentNullException(nameof(urlToLicenseTypeMappingProviders));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        _urlToLicenseTypeMappings = new Dictionary<string, string>();
    }

    // somewhere at the end as a "fallback" for anything that has no license-type yet
    public int Order => 15;

    public Task GetInformationAsync(GetLicenseInformationContext licenseInformationContext)
    {
        if (licenseInformationContext == null)
        {
            throw new ArgumentNullException(nameof(licenseInformationContext));
        }

        Initialize();

        var licenseUrl = licenseInformationContext.LicenseInformation.Url;

        // no need to attempt getting license-type from url, when there's no url, duh
        if (string.IsNullOrWhiteSpace(licenseUrl))
        {
            return Task.CompletedTask;
        }

        _logger.LogDebug($"Attempting to get license-type from license-url for '{licenseUrl}'");

        HandleLicenseUrl(licenseInformationContext, licenseUrl);

        return Task.CompletedTask;
    }

    private void HandleLicenseUrl(GetLicenseInformationContext context, string licenseUrl)
    {
        if (!_urlToLicenseTypeMappings.TryGetValue(licenseUrl, out var mappedType) ||
            string.IsNullOrWhiteSpace(mappedType))
        {
            return;
        }

        context.LicenseInformation.AddLicenseType(mappedType);
    }

    /*
     * NOTE:
     * We need this here, because some mappings come from the settings which are partly set by a preprocessor,
     * which has not been executed yet, when the constructor is called
     */
    private void Initialize()
    {
        if (_isInitialized)
        {
            return;
        }

        _urlToLicenseTypeMappings = GetMappings();
        _isInitialized = true;
    }

    /*
     * Basically merging all the dictionaries together
     * using "TryAdd" to ignore any duplicate keys and their corresponding exceptions
     */
    private Dictionary<string, string> GetMappings()
    {
        var mappings = new Dictionary<string, string>();

        foreach (var provider in _urlToLicenseTypeMappingProviders)
        {
            var providerMappings = provider.Get();

            foreach (var providerMapping in providerMappings)
            {
                _ = mappings.TryAdd(providerMapping.Key, providerMapping.Value);
            }
        }

        return mappings;
    }
}
