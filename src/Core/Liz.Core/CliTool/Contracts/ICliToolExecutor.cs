namespace Liz.Core.CliTool.Contracts;

internal interface ICliToolExecutor
{
    Task<string> ExecuteWithResultAsync(string fileName, string arguments);
    Task ExecuteAsync(string fileName, string arguments);
}
