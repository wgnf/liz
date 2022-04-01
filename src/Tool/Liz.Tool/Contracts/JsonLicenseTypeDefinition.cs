using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

// because this is implicitly used by the JSON-Deserializer
// ReSharper disable ClassNeverInstantiated.Global
// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global

namespace Liz.Tool.Contracts;

[ExcludeFromCodeCoverage] // simple DTO
public sealed class JsonLicenseTypeDefinition
{
    [JsonPropertyName("type")]
    public string LicenseType { get; set; } = string.Empty;

    [JsonPropertyName("inclusiveText")]
    public IEnumerable<string> InclusiveTextSnippets { get; set; } = Enumerable.Empty<string>();

    [JsonPropertyName("exclusiveText")]
    public IEnumerable<string> ExclusiveTextSnippets { get; set; } = Enumerable.Empty<string>();
}
