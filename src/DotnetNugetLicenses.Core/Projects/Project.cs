using System.IO.Abstractions;

namespace DotnetNugetLicenses.Core.Projects;

internal sealed record Project(string Name, IFileInfo File);
