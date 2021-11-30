using Liz.Tool.CommandLine;
using System.CommandLine;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

namespace Liz.Tool;

[ExcludeFromCodeCoverage] // mostly untestable startup code
public static class Program
{
    public static Task<int> Main(string[] args)
    {
        var commandProvider = new CommandProvider();
        var rootCommand = commandProvider.Get();

        return rootCommand.InvokeAsync(args);
    }
}
