using Liz.Core.Settings;
using System.Diagnostics.CodeAnalysis;

namespace Liz.Tool;

[ExcludeFromCodeCoverage] // simple settings DTO
public class ExtractLicensesSettings : ExtractLicensesSettingsBase
{
    public string? TargetFile { get; init; }

    public override string? GetTargetFile()
    {
        return TargetFile;
    }
}
