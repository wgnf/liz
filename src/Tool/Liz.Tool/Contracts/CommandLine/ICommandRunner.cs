using Liz.Core.Logging;
using System.IO;
using System.Threading.Tasks;

namespace Liz.Tool.Contracts.CommandLine;

public interface ICommandRunner
{
    Task RunAsync(FileInfo targetFile, LogLevel logLevel, bool includeTransitive);
}
