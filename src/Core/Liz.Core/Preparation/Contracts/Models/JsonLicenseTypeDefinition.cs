using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace Liz.Core.Preparation.Contracts.Models;

[ExcludeFromCodeCoverage] // simple DTO
internal sealed class JsonLicenseTypeDefinition
{
    [JsonPropertyName("type")]
    public string LicenseType { get; set; } = string.Empty;

    [JsonPropertyName("inclusiveText")]
    public IEnumerable<string> InclusiveTextSnippets { get; set; } = Enumerable.Empty<string>();

    [JsonPropertyName("exclusiveText")]
    public IEnumerable<string> ExclusiveTextSnippets { get; set; } = Enumerable.Empty<string>();
}
