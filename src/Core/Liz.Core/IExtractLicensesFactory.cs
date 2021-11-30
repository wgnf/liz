using Liz.Core.Extract;
using Liz.Core.Logging;
using Liz.Core.Settings;

namespace Liz.Core;

public interface IExtractLicensesFactory
{
    IExtractLicenses Create(ExtractLicensesSettings settings, ILoggerProvider loggerProvider = null);
}
