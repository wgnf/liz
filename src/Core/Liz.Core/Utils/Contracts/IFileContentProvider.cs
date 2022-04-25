namespace Liz.Core.Utils.Contracts;

internal interface IFileContentProvider
{
    Task<string> GetFileContentAsync(string filePath);
}
