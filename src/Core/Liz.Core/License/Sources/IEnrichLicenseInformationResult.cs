using System.Threading.Tasks;

namespace Liz.Core.License.Sources;

internal interface IEnrichLicenseInformationResult
{
    int Order { get; }

    Task EnrichAsync(GetLicenseInformationResult licenseInformationResult);
}
