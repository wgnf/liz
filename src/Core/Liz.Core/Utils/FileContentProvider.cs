using Liz.Core.Utils.Contracts;
using Liz.Core.Utils.Contracts.Wrappers;
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
            contentOfFile = await GetContentOfFileAsync(filePath).ConfigureAwait(false);
        else if (fileUri.IsFile)
            contentOfFile = await GetContentOfFileAsync(fileUri.LocalPath).ConfigureAwait(false);
        else
        {
            var downloadedFilePath = await DownloadRemoteFileAsync(filePath).ConfigureAwait(false);
            contentOfFile = await GetContentOfFileAsync(downloadedFilePath).ConfigureAwait(false);
        }

        return contentOfFile;
    }

    private async Task<string> GetContentOfFileAsync(string filePath)
    {
        var fileContent = await _fileSystem.File.ReadAllTextAsync(filePath).ConfigureAwait(false);
        return fileContent;
    }

    private async Task<string> DownloadRemoteFileAsync(string remoteFilePath)
    {
        var response = await _httpClient.GetAsync(remoteFilePath).ConfigureAwait(false);
        response.EnsureSuccessStatusCode();

        var tempFile = _fileSystem.Path.GetTempFileName();
        await using var fileStream = _fileSystem
            .FileStream
            .Create(tempFile, FileMode.Create, FileAccess.Write);

        await response.Content.CopyToAsync(fileStream).ConfigureAwait(false);

        return tempFile;
    }
}
