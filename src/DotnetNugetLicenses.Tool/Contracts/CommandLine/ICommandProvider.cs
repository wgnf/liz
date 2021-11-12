using System.CommandLine;

namespace DotnetNugetLicenses.Tool.Contracts.CommandLine
{
    public interface ICommandProvider
    {
        RootCommand Get();
    }
}
