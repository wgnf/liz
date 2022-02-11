using Liz.Core.PackageReferences.Contracts.Models;
using Liz.Core.Projects.Contracts.Models;
using System;
using System.Diagnostics.CodeAnalysis;

namespace Liz.Core.PackageReferences.Contracts.Exceptions;

/// <summary>
///     <see cref="Exception" /> that occurs when the tool failed to determine <see cref="PackageReference"/>s for a given
///     <see cref="Project" />
/// </summary>
[ExcludeFromCodeCoverage] // simple exception
public sealed class GetPackageReferencesFailedException : Exception
{
    /// <summary>
    ///     Create a new instance of <see cref="GetPackageReferencesFailedException" />
    /// </summary>
    /// <param name="project">The target project which was used to determine the <see cref="PackageReference"/>s</param>
    /// <param name="innerException">An optional inner <see cref="Exception" /></param>
    internal GetPackageReferencesFailedException(Project project, Exception? innerException = null)
        : base($"Unable to determine project-references for target project {project}", innerException)
    {
    }
}
