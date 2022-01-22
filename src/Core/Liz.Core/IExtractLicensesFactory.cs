using Liz.Core.Extract.Contracts;
using Liz.Core.Logging.Contracts;
using Liz.Core.Settings;

namespace Liz.Core;

public interface IExtractLicensesFactory
{
    IExtractLicenses Create(ExtractLicensesSettings settings, ILoggerProvider loggerProvider = null);
}
