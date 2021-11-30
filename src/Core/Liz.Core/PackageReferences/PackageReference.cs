using System.Diagnostics.CodeAnalysis;

namespace Liz.Core.PackageReferences;

[ExcludeFromCodeCoverage] // DTO
internal sealed record PackageReference(string Name, string TargetFramework, string Version);
