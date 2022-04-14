using Liz.Core.Utils.Contracts;
using Liz.Core.Utils.Contracts.Wrappers;
using System.Diagnostics.CodeAnalysis;
using System.IO.Abstractions;

namespace Liz.Core.Utils;

internal sealed class FileContentProvider : IFileContentProvider
{
    private readonly IFileSystem _fileSystem;
    private readonly IHttpClient _httpClient;

    public FileContentProvider(IFileSystem fileSystem, IHttpClient httpClient)
    {
        _fileSystem = fileSystem ?? throw new ArgumentNullException(nameof(fileSystem));
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
    }
    
    public async Task<string> GetFileContentAsync(string filePath)
    {
        if (string.IsNullOrWhiteSpace(filePath))
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(filePath));

        var fileUri = new Uri(filePath, UriKind.RelativeOrAbsolute);

        string contentOfFile;
        
        // when it's a relative path it's gotta be a local file path
        if (!fileUri.IsAbsoluteUri)
            contentOfFile = await GetContentOfFileAsync(filePath);
        else if (fileUri.IsFile)
            contentOfFile = await GetContentOfFileAsync(fileUri.LocalPath);
        else
        {
            var downloadedFilePath = await DownloadRemoteFileAsync(filePath);
            contentOfFile = await GetContentOfFileAsync(downloadedFilePath);
        }

        return contentOfFile;
    }

    private async Task<string> GetContentOfFileAsync(string filePath)
    {
        var fileContent = await _fileSystem.File.ReadAllTextAsync(filePath);
        return fileContent;
    }

    [ExcludeFromCodeCoverage] // not sure how to test this properly (apart from the existing integration test)
    private async Task<string> DownloadRemoteFileAsync(string remoteFilePath)
    {
        var response = await _httpClient.GetAsync(remoteFilePath);
        response.EnsureSuccessStatusCode();

        var tempFile = _fileSystem.Path.GetTempFileName();
        await using var fileStream = _fileSystem.FileStream.Create(tempFile, FileMode.Create, FileAccess.Write);

        await response.Content.CopyToAsync(fileStream);

        return tempFile;
    }
}
