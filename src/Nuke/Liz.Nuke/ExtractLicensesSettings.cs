using Nuke.Common.IO;
using System.Diagnostics.CodeAnalysis;

// ReSharper disable UnusedAutoPropertyAccessor.Global -- public API

namespace Liz.Nuke;

/// <summary>
///     Settings to parameterize the license extraction of the Liz-Tool
/// </summary>
[ExcludeFromCodeCoverage] // simple settings
public sealed class ExtractLicensesSettings
{
    /// <summary>
    ///     The target-file to analyze. Can be a .csproj, .fsproj and .sln
    /// </summary>
    public AbsolutePath? TargetFile { get; set; }
    
    /// <summary>
    ///     Whether or not to include transitive (dependencies of dependencies) dependencies
    /// </summary>
    public bool IncludeTransitiveDependencies { get; set; }
    
    /// <summary>
    ///     Whether or not to suppress printing details of analyzed package-references and license-information
    /// </summary>
    public bool SuppressPrintDetails { get; set; }
    
    /// <summary>
    ///     Whether or not to suppress printing found issues of analyzed package-references and license-information
    /// </summary>
    public bool SuppressPrintIssues { get; set; }
}
