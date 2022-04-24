using Liz.Core.License.Contracts;
using Liz.Core.License.Contracts.Models;
using Liz.Core.Logging;
using Liz.Core.Logging.Contracts;

namespace Liz.Core.License.Sources.LicenseInformation;

internal sealed class UrlToLicenseTypeMappingLicenseInformationSource : ILicenseInformationSource
{
    private readonly Dictionary<string, string> _urlToLicenseTypeMappings;
    private readonly ILogger _logger;

    public UrlToLicenseTypeMappingLicenseInformationSource(
        IEnumerable<IUrlToLicenseTypeMappingProvider> urlToLicenseTypeMappingProviders,
        ILogger logger)
    {
        if (urlToLicenseTypeMappingProviders == null)
            throw new ArgumentNullException(nameof(urlToLicenseTypeMappingProviders));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        _urlToLicenseTypeMappings = GetMappings(urlToLicenseTypeMappingProviders);
    }
    
    // somewhere at the end as a "fallback" for anything that has no license-type yet
    public int Order => 15;
    
    public Task GetInformationAsync(GetLicenseInformationContext licenseInformationContext)
    {
        if (licenseInformationContext == null) throw new ArgumentNullException(nameof(licenseInformationContext));

        var licenseUrl = licenseInformationContext.LicenseInformation.Url;
        
        // no need to attempt getting license-type from url, when there's no url, duh
        if (string.IsNullOrWhiteSpace(licenseUrl)) return Task.CompletedTask;
        
        _logger.LogDebug($"Attempting to get license-type from license-url for '{licenseUrl}'");
        
        HandleLicenseUrl(licenseInformationContext, licenseUrl);
        
        return Task.CompletedTask;
    }

    private void HandleLicenseUrl(GetLicenseInformationContext context, string licenseUrl)
    {
        if (!_urlToLicenseTypeMappings.TryGetValue(licenseUrl, out var mappedType) || 
            string.IsNullOrWhiteSpace(mappedType)) return;
        
        context.LicenseInformation.AddLicenseType(mappedType);
    }

    /*
     * Basically merging all the dictionaries together
     * using "TryAdd" to ignore any duplicate keys and their corresponding exceptions
     */
    private static Dictionary<string, string> GetMappings(
        IEnumerable<IUrlToLicenseTypeMappingProvider> urlToLicenseTypeMappingProviders)
    {
        var mappings = new Dictionary<string, string>();

        foreach (var provider in urlToLicenseTypeMappingProviders)
        {
            var providerMappings = provider.Get();

            foreach (var providerMapping in providerMappings)
                _ = mappings.TryAdd(providerMapping.Key, providerMapping.Value);
        }
        
        return mappings;
    }
}
