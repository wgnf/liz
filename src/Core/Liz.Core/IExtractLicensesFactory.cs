using Liz.Core.Extract.Contracts;
using Liz.Core.Logging.Contracts;
using Liz.Core.Progress;
using Liz.Core.Settings;

namespace Liz.Core;

/// <summary>
///     Factory to create an instance of <see cref="IExtractLicenses"/>
/// </summary>
public interface IExtractLicensesFactory
{
    /// <summary>
    ///     Create an instance of <see cref="IExtractLicenses"/> with the given <paramref name="settings"/>
    /// </summary>
    /// <param name="settings">The <see cref="ExtractLicensesSettings"/> which are configuring the desired instance</param>
    /// <param name="loggerProvider">An optional <see cref="ILoggerProvider"/> to log messages from the tool</param>
    /// <param name="progressHandler">An optional <see cref="IProgressHandler"/> that can handle process notifications</param>
    /// <returns>The created instance of <see cref="IExtractLicenses"/></returns>
    IExtractLicenses Create(
        ExtractLicensesSettings settings, 
        ILoggerProvider? loggerProvider = null, 
        IProgressHandler? progressHandler = null);
}
