using Liz.Core.License;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace Liz.Core.PackageReferences;

[ExcludeFromCodeCoverage] // DTO
// internal sealed record PackageReference(string Name, string TargetFramework, string Version);
internal sealed class PackageReference
{
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

    public string Name { get; }

    public string TargetFramework { get; }

    public string Version { get; }

    public LicenseInformation LicenseInformation { get; set; }

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
