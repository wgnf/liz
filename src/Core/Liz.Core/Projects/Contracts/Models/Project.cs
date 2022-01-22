using System.IO.Abstractions;

namespace Liz.Core.Projects.Contracts.Models;

internal sealed record Project(string Name, IFileInfo File, ProjectFormatStyle FormatStyle);
