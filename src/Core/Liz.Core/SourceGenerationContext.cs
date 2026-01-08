using System.Text.Json.Serialization;
using Liz.Core.PackageReferences.Contracts.Models;
using Liz.Core.Preparation.Contracts.Models;

namespace Liz.Core;

[JsonSourceGenerationOptions(WriteIndented = true)]
[JsonSerializable(typeof(List<string>))]
[JsonSerializable(typeof(Dictionary<string, string>))]

[JsonSerializable(typeof(JsonLicenseTypeDefinition))]
[JsonSerializable(typeof(List<JsonLicenseTypeDefinition>))]

[JsonSerializable(typeof(PackageReference))]
[JsonSerializable(typeof(IEnumerable<PackageReference>))]
internal partial class SourceGenerationContext : JsonSerializerContext
{
}
