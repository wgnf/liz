using Liz.Core.Settings;
using System.Diagnostics.CodeAnalysis;

namespace Liz.Tool;

[ExcludeFromCodeCoverage] // simple settings DTO
public class ExtractLicensesSettings : ExtractLicensesSettingsBase
{
    public ExtractLicensesSettings(string targetFile)
    {
        TargetFile = targetFile;
    }
    
    public string TargetFile { get; }

    public override string GetTargetFile()
    {
        return TargetFile;
    }
}
