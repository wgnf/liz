using System.Diagnostics.CodeAnalysis;
using System.IO.Abstractions;

namespace Liz.Core.Projects.Contracts.Models;

[ExcludeFromCodeCoverage] // DTO
internal sealed record Project(string Name, IFileInfo File, ProjectFormatStyle FormatStyle)
{
    public string Name { get; } = Name;
    
    public IFileInfo File { get; } = File;
    
    public ProjectFormatStyle FormatStyle { get; } = FormatStyle;

    public override string ToString()
    {
        return $"{Name} ('{File.FullName}')";
    }
}
