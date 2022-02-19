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
    /// <param name="settings">The <see cref="ExtractLicensesSettingsBase"/> which are configuring the desired instance</param>
    /// <param name="loggerProvider">An optional <see cref="ILoggerProvider"/> to log messages from the tool</param>
    /// <param name="logLevel">The log level on which to log on (default: <see cref="F:LogLevel.Information"/>)</param>
    /// <param name="progressHandler">An optional <see cref="IProgressHandler"/> that can handle process notifications</param>
    /// <returns>The created instance of <see cref="IExtractLicenses"/></returns>
    IExtractLicenses Create(
        ExtractLicensesSettingsBase settings, 
        ILoggerProvider? loggerProvider = null,
        LogLevel logLevel = LogLevel.Information,
        IProgressHandler? progressHandler = null);
}
