using Liz.Core.CliTool.Contracts;
using Liz.Core.Utils.Contracts;
using System.IO.Abstractions;

namespace Liz.Core.Utils;

internal sealed class ProvideNugetCacheDirectories : IProvideNugetCacheDirectories
{
    private readonly ICliToolExecutor _cliToolExecutor;
    private readonly IFileSystem _fileSystem;

    private IEnumerable<string>? _nugetCacheDirectories;

    public ProvideNugetCacheDirectories(
        ICliToolExecutor cliToolExecutor,
        IFileSystem fileSystem)
    {
        _cliToolExecutor = cliToolExecutor ?? throw new ArgumentNullException(nameof(cliToolExecutor));
        _fileSystem = fileSystem ?? throw new ArgumentNullException(nameof(fileSystem));
    }

    public async Task<IEnumerable<string>> GetAsync()
    {
        _nugetCacheDirectories ??= await GetCacheDirectoriesAsync().ConfigureAwait(false);
        return _nugetCacheDirectories;
    }

    private async Task<IEnumerable<string>> GetCacheDirectoriesAsync()
    {
        var cacheDirectories = await GetNugetLocalsGlobalPackagesAsync();

        if (TryGetNugetFallbackFolder(out var nugetFallbackFolder)) 
            cacheDirectories.Add(nugetFallbackFolder);
        
        return cacheDirectories;
    }

    private async Task<IList<string>> GetNugetLocalsGlobalPackagesAsync()
    {
        // refer to https://docs.microsoft.com/en-us/dotnet/core/tools/dotnet-nuget-locals
        var result = await _cliToolExecutor
            .ExecuteWithResultAsync("dotnet", "nuget locals global-packages --list --force-english-output")
            .ConfigureAwait(false);
        var cacheDirectories = ParseCliOutput(result);

        return cacheDirectories;
    }

    private bool TryGetNugetFallbackFolder(out string nugetFallbackFolder)
    {
        var programFiles = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles);
        nugetFallbackFolder = _fileSystem.Path.Combine(programFiles, "dotnet", "sdk", "NuGetFallbackFolder");

        return _fileSystem.Directory.Exists(nugetFallbackFolder);
    }

    private static IList<string> ParseCliOutput(string cliOutput)
    {
        const string searchString = "global-packages: ";

        var cacheDirectories = cliOutput
            .Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries)
            .Select(line => line.Trim())
            .Select(line =>
            {
                var indexOfSearchString = line.IndexOf(searchString, StringComparison.InvariantCultureIgnoreCase);
                // we want to get the stuff from the search string to the end
                var directory = line[(indexOfSearchString + searchString.Length)..];

                return directory;
            })
            .ToList();

        return cacheDirectories;
    }
}
