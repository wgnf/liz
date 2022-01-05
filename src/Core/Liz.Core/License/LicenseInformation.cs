using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace Liz.Core.License;

[ExcludeFromCodeCoverage] // DTO
internal sealed class LicenseInformation
{
    public string Type { get; set; }

    public string Url { get; set; }

    public string Text { get; set; }

    public override string ToString()
    {
        var builder = new StringBuilder();

        builder.Append(nameof(LicenseInformation));
        builder.Append(" { ");
        builder.Append($"{nameof(Type)}={Type}, ");
        builder.Append($"{nameof(Url)}={Url}");
        // leaving out 'Text' because it can be quite long
        builder.Append(" }");

        var objectString = builder.ToString();
        return objectString;
    }
}
