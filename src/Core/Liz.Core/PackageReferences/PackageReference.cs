using System.Diagnostics.CodeAnalysis;

namespace DotnetNugetLicenses.Core.PackageReferences;

[ExcludeFromCodeCoverage] // DTO
internal sealed record PackageReference(string Name, string TargetFramework, string Version);
