﻿using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace Liz.Core.License.Contracts.Models;

/// <summary>
///     An object containing information about a license
/// </summary>
[ExcludeFromCodeCoverage] // DTO
public sealed class LicenseInformation
{
    /// <summary>
    ///     Creates a new instance of <see cref="LicenseInformation"/>
    /// </summary>
    public LicenseInformation()
    {
        Type = string.Empty;
        Url = string.Empty;
        Text = string.Empty;
    }
    
    /// <summary>
    ///     The type of a license
    /// </summary>
    /// <example>MIT</example>
    public string Type { get; set; }

    /// <summary>
    ///     The URL of the license
    /// </summary>
    /// <example>https://licenses.nuget.org/MIT</example>
    public string Url { get; set; }

    /// <summary>
    ///     The raw license text of that license
    /// </summary>
    public string Text { get; set; }

    /// <summary>
    ///     Provides a string that represents this instance
    /// </summary>
    /// <returns>A string representing this instance</returns>
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
    
    internal bool IsEmpty()
    {
        return string.IsNullOrWhiteSpace(Type) &&
               string.IsNullOrWhiteSpace(Url) &&
               string.IsNullOrWhiteSpace(Url);
    }
}
