﻿using Liz.Core.License.Contracts.Models;
using Liz.Core.Projects.Contracts.Models;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace Liz.Core.PackageReferences.Contracts.Models;

/// <summary>
///     Information about a dependency of a <see cref="Project"/>
/// </summary>
[ExcludeFromCodeCoverage] // DTO
public sealed class PackageReference : IEquatable<PackageReference>
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
    public PackageReference(string name, string targetFramework, string version)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(name));
        }

        if (string.IsNullOrWhiteSpace(targetFramework))
        {
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(targetFramework));
        }

        if (string.IsNullOrWhiteSpace(version))
        {
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(version));
        }

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
    ///     The path to the directory where the artifacts of this package are, if available
    /// </summary>
    /// <example>C:/Users/User/.nuget/packages/system.io/4.3.0</example>
    public string? ArtifactDirectory { get; set; }

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
        builder.Append($"{nameof(ArtifactDirectory)}={ArtifactDirectory}, ");
        builder.Append($"{{ {nameof(LicenseInformation)}={LicenseInformation} }}");
        builder.Append(" }");

        var objectString = builder.ToString();
        return objectString;
    }

    /// <inheritdoc />
    public bool Equals(PackageReference? other)
    {
        if (ReferenceEquals(null, other))
        {
            return false;
        }

        if (ReferenceEquals(this, other))
        {
            return true;
        }

        return string.Equals(Name, other.Name, StringComparison.InvariantCultureIgnoreCase) && string.Equals(Version, other.Version, StringComparison.InvariantCultureIgnoreCase);
    }

    /// <inheritdoc />
    public override bool Equals(object? obj)
    {
        return ReferenceEquals(this, obj) || obj is PackageReference other && Equals(other);
    }

    /// <inheritdoc />
    public override int GetHashCode()
    {
        var hashCode = new HashCode();
        hashCode.Add(Name, StringComparer.InvariantCultureIgnoreCase);
        hashCode.Add(Version, StringComparer.InvariantCultureIgnoreCase);
        return hashCode.ToHashCode();
    }

    /// <summary>
    ///     Indicates whether the <paramref name="left"/> object is equal to the <paramref name="right"/> object
    /// </summary>
    /// <param name="left">The first object to compare</param>
    /// <param name="right">The second object to compare</param>
    /// <returns>Whether or not the objects are equal to each other</returns>
    public static bool operator ==(PackageReference? left, PackageReference? right)
    {
        return Equals(left, right);
    }

    /// <summary>
    ///     Indicates whether the <paramref name="left"/> object is not equal to the <paramref name="right"/> object
    /// </summary>
    /// <param name="left">The first object to compare</param>
    /// <param name="right">The second object to compare</param>
    /// <returns>Whether or not the objects are not equal to each other</returns>
    public static bool operator !=(PackageReference? left, PackageReference? right)
    {
        return !Equals(left, right);
    }
}
