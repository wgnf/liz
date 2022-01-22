using Liz.Core.License.Contracts.Models;
using Liz.Core.Projects.Contracts.Models;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace Liz.Core.PackageReferences.Contracts.Models;

/// <summary>
///     Information about a dependency of a <see cref="Project"/>
/// </summary>
[ExcludeFromCodeCoverage] // DTO
public sealed class PackageReference
{
    /// <summary>
    ///     Create a new instance of <see cref="PackageReference"/>
    /// </summary>
    /// <param name="name">The name of the package</param>
    /// <param name="targetFramework">The framework that the reference targets</param>
    /// <param name="version">The version that is being targeted</param>
    /// <exception cref="ArgumentException">
    ///     All parameters are mandatory so an Exception will be thrown when any of those are not provided
    /// </exception>
    public PackageReference(
        [JetBrains.Annotations.NotNull] string name,
        [JetBrains.Annotations.NotNull] string targetFramework,
        [JetBrains.Annotations.NotNull] string version)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(name));
        if (string.IsNullOrWhiteSpace(targetFramework))
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(targetFramework));
        if (string.IsNullOrWhiteSpace(version))
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(version));

        Name = name;
        TargetFramework = targetFramework;
        Version = version;

        LicenseInformation = new LicenseInformation();
    }
    
    /// <summary>
    ///     The name of the package
    /// </summary>
    /// <example>Newtonsoft.Json</example>
    public string Name { get; }

    /// <summary>
    ///     The framework that the reference targets
    /// </summary>
    /// <example>net6.0</example>
    public string TargetFramework { get; }

    /// <summary>
    ///     The version that is being targeted
    /// </summary>
    /// <example>11.1.0</example>
    public string Version { get; }

    /// <summary>
    ///     The acquired <see cref="LicenseInformation"/> of this package 
    /// </summary>
    public LicenseInformation LicenseInformation { get; set; }

    /// <summary>
    ///     Provides a string that represents this instance
    /// </summary>
    /// <returns>A string representing this instance</returns>
    public override string ToString()
    {
        var builder = new StringBuilder();

        builder.Append(nameof(PackageReference));
        builder.Append(" { ");
        builder.Append($"{nameof(Name)}={Name}, ");
        builder.Append($"{nameof(TargetFramework)}={TargetFramework}, ");
        builder.Append($"{nameof(Version)}={Version}, ");
        builder.Append($"{{ {nameof(LicenseInformation)}={LicenseInformation} }}");
        builder.Append(" }");

        var objectString = builder.ToString();
        return objectString;
    }
}
