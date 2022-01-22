using JetBrains.Annotations;
using Liz.Core.Logging.Contracts;
using System;
using System.Diagnostics.CodeAnalysis;

namespace Liz.Core.Settings;

[PublicAPI]
[ExcludeFromCodeCoverage] // simple DTO
public sealed class ExtractLicensesSettings
{
    public ExtractLicensesSettings(string targetFile)
    {
        if (string.IsNullOrWhiteSpace(targetFile))
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(targetFile));
        
        TargetFile = targetFile;
    }

    public string TargetFile { get; }
    
    public bool IncludeTransitiveDependencies { get; init; }

    public LogLevel LogLevel { get; init; } = LogLevel.Information;
}
