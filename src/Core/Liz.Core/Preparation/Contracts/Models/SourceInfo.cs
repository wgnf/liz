using System.Diagnostics.CodeAnalysis;

namespace Liz.Core.Preparation.Contracts.Models;

/// <summary>
///     Simple info object containing information about the source code that is currently analyzed
/// </summary>
[ExcludeFromCodeCoverage] // simple DTO
internal sealed class SourceInfo
{
    /// <summary>
    ///     Whether or not the source-code is managed using Central Package Management (CPM)
    /// </summary>
    public bool IsCpmEnabled { get; set; }
}
