using System.IO.Abstractions;

namespace Liz.Core.Projects;

internal sealed record Project(string Name, IFileInfo File, ProjectFormatStyle FormatStyle);
