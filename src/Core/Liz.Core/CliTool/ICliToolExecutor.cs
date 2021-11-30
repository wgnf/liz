using JetBrains.Annotations;
using System.Threading.Tasks;

namespace Liz.Core.CliTool;

internal interface ICliToolExecutor
{
    Task<string> ExecuteWithResultAsync([NotNull] string fileName, [NotNull] string arguments);
    Task ExecuteAsync([NotNull] string fileName, [NotNull] string arguments);
}
