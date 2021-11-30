using System.CommandLine;

namespace Liz.Tool.Contracts.CommandLine;

public interface ICommandProvider
{
    RootCommand Get();
}
