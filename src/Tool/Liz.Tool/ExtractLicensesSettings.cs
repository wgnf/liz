using Liz.Core.Settings;
using System.Diagnostics.CodeAnalysis;

namespace Liz.Tool;

[ExcludeFromCodeCoverage] // simple settings DTO
public class ExtractLicensesSettings : ExtractLicensesSettingsBase
{
    private readonly string _targetFile;

    public ExtractLicensesSettings(string targetFile)
    {
        _targetFile = targetFile;
    }

    public override string GetTargetFile()
    {
        return _targetFile;
    }
}
