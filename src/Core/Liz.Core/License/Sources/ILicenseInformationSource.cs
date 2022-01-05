using System.Threading.Tasks;

namespace Liz.Core.License.Sources;

internal interface ILicenseInformationSource
{
    int Order { get; }

    Task GetInformationAsync(GetLicenseInformationContext licenseInformationContext);
}
