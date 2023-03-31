using Liz.Core.License.Contracts;
using Liz.Core.License.Contracts.Models;
using Liz.Core.Settings;

namespace Liz.Core.License.Sources.LicenseType;

internal sealed class LicenseTypeDefinitionFromSettingsProvider : ILicenseTypeDefinitionProvider
{
    private readonly ExtractLicensesSettingsBase _settings;

    public LicenseTypeDefinitionFromSettingsProvider(ExtractLicensesSettingsBase settings)
    {
        _settings = settings ?? throw new ArgumentNullException(nameof(settings));
    }

    public IEnumerable<LicenseTypeDefinition> Get()
    {
        return _settings.LicenseTypeDefinitions;
    }
}
