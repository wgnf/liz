using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

namespace Liz.Core.CliTool.Contracts;

internal interface ICliToolExecutor
{
    Task<string> ExecuteWithResultAsync([NotNull] string fileName, [NotNull] string arguments);
    Task ExecuteAsync([NotNull] string fileName, [NotNull] string arguments);
}
