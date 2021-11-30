using DotnetNugetLicenses.Core.Extract;
using DotnetNugetLicenses.Core.Logging;
using DotnetNugetLicenses.Core.Settings;

namespace DotnetNugetLicenses.Core;

public interface IExtractLicensesFactory
{
    IExtractLicenses Create(ExtractLicensesSettings settings, ILoggerProvider loggerProvider = null);
}
