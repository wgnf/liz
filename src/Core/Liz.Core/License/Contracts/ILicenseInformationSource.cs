using Liz.Core.License.Contracts.Models;

namespace Liz.Core.License.Contracts;

internal interface ILicenseInformationSource
{
    int Order { get; }

    Task GetInformationAsync(GetLicenseInformationContext licenseInformationContext);
}
