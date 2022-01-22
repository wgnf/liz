using Liz.Core.Projects.Contracts.Models;
using System;
using System.Diagnostics.CodeAnalysis;

namespace Liz.Core.Projects.Contracts.Exceptions;

/// <summary>
///     <see cref="Exception" /> that occurs when the tool failed to determine <see cref="Project"/>s for a provided file
/// </summary>
[ExcludeFromCodeCoverage] // simple exception
public sealed class GetProjectsFailedException : Exception
{
    /// <summary>
    ///     Create a new instance of <see cref="GetProjectsFailedException" />
    /// </summary>
    /// <param name="targetFile">The target-file which was used to get the projects</param>
    /// <param name="innerException">An optional inner <see cref="Exception" /></param>
    public GetProjectsFailedException(string targetFile, Exception innerException = null)
        : base($"Unable to determine projects for target-file '{targetFile}'", innerException)
    {
    }
}
