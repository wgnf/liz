using Liz.Core.License.Contracts;
using Liz.Core.Settings;

namespace Liz.Core.License.Sources.UrlToLicenseType;

internal sealed class UrlToLicenseTypeFromSettingsProvider : IUrlToLicenseTypeMappingProvider
{
    private readonly ExtractLicensesSettingsBase _settings;

    public UrlToLicenseTypeFromSettingsProvider(ExtractLicensesSettingsBase settings)
    {
        _settings = settings ?? throw new ArgumentNullException(nameof(settings));
    }
    
    public IDictionary<string, string> Get()
    {
        return _settings.UrlToLicenseTypeMapping;
    }
}
