using Cake.Core.IO;
using Liz.Core.Settings;
using System.Diagnostics.CodeAnalysis;

// public API...
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable ClassNeverInstantiated.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace Liz.Cake;

/// <inheritdoc />
[ExcludeFromCodeCoverage] // simple settings DTO
public class ExtractLicensesSettings : ExtractLicensesSettingsBase
{
    /// <summary>
    ///     The target file that needs to be analyzed - can be a csproj, fsproj and sln file
    /// </summary>
    public FilePath? TargetFile { get; set; }

    /// <inheritdoc />
    public override string GetTargetFile()
    {
        return TargetFile?.FullPath ?? string.Empty;
    }
}
