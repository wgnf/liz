﻿using Liz.Core.Settings;
using Nuke.Common.IO;
using System.Diagnostics.CodeAnalysis;

// ReSharper disable UnusedAutoPropertyAccessor.Global -- public API

namespace Liz.Nuke;

/// <inheritdoc />
[ExcludeFromCodeCoverage] // simple settings
public sealed class ExtractLicensesSettings : ExtractLicensesSettingsBase
{
    /// <summary>
    ///     The target file that needs to be analyzed - can be a csproj, fsproj and sln file
    /// </summary>
    public AbsolutePath? TargetFile { get; set; }

    /// <inheritdoc />
    public override string GetTargetFile()
    {
        return TargetFile?.ToString() ?? string.Empty;
    }
}
