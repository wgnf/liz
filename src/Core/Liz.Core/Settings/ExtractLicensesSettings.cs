using Liz.Core.Logging.Contracts;
using Microsoft.VisualBasic;
using System.Diagnostics.CodeAnalysis;

namespace Liz.Core.Settings;

/// <summary>
///     Settings to configure the Extract-Licenses-Tool aka Liz
/// </summary>
[ExcludeFromCodeCoverage] // simple DTO
public sealed class ExtractLicensesSettings
{
    /// <summary>
    ///     Create a new instance of <see cref="ExtractLicensesSettings"/>
    /// </summary>
    /// <param name="targetFile">The target file that needs to be analyzed - can be a csproj, fsproj and sln file</param>
    /// <exception cref="ArgumentException">Thrown when a <paramref name="targetFile"/> has not been given</exception>
    public ExtractLicensesSettings(string targetFile)
    {
        if (string.IsNullOrWhiteSpace(targetFile))
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(targetFile));
        
        TargetFile = targetFile;
    }

    /// <summary>
    ///     The target file that needs to be analyzed - can be a csproj, fsproj and sln file
    /// </summary>
    public string TargetFile { get; }
    
    /// <summary>
    ///     Whether or not to include transitive (dependencies of dependencies) dependencies
    /// </summary>
    public bool IncludeTransitiveDependencies { get; init; }

    /// <summary>
    ///     The log level on which to log on (default: <see cref="Information"/>)
    /// </summary>
    public LogLevel LogLevel { get; init; } = LogLevel.Information;
    
    /// <summary>
    ///     Whether or not to suppress printing details of analyzed package-references and license-information
    /// </summary>
    public bool SuppressPrintDetails { get; init; }
    
    /// <summary>
    ///     Whether or not to suppress printing found issues of analyzed package-references and license-information
    /// </summary>
    public bool SuppressPrintIssues { get; init; }
}
