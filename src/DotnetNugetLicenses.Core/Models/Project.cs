using System.IO.Abstractions;

namespace DotnetNugetLicenses.Core.Models
{
    public sealed record Project(string Name, IFileInfo File);
}
