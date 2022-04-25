using Liz.Core.License.Contracts;

// ReSharper disable StringLiteralTypo

namespace Liz.Core.License.Sources.UrlToLicenseType;

internal sealed class MicrosoftUrlToLicenseTypeProvider : IUrlToLicenseTypeMappingProvider
{
    public IDictionary<string, string> Get()
    {
        return new Dictionary<string, string>
        {
            { "http://www.microsoft.com/web/webpi/eula/aspnetcomponent_rtw_enu.htm", "MS-NETLIB" },
            { "https://www.microsoft.com/web/webpi/eula/aspnetcomponent_rtw_enu.htm", "MS-NETLIB" },
            { "http://www.microsoft.com/web/webpi/eula/aspnetcomponent_rtw_ENU.htm", "MS-NETLIB" },
            { "https://www.microsoft.com/web/webpi/eula/aspnetcomponent_rtw_ENU.htm", "MS-NETLIB" },
            { "https://go.microsoft.com/fwlink/?linkid=841311", "MIT" }
        };
    }
}
