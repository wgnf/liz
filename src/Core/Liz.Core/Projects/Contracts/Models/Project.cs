using System.IO.Abstractions;

namespace Liz.Core.Projects.Contracts.Models;

internal sealed record Project(string Name, IFileInfo File, ProjectFormatStyle FormatStyle)
{
    public string Name { get; } = Name;
    
    public IFileInfo File { get; } = File;
    
    public ProjectFormatStyle FormatStyle { get; } = FormatStyle;
}
