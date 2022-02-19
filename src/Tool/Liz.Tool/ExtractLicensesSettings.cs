using Liz.Core.Settings;

namespace Liz.Tool;

public class ExtractLicensesSettings : ExtractLicensesSettingsBase
{
    public string? TargetFile { get; set; }

    public override string? GetTargetFile()
    {
        return TargetFile;
    }
}
