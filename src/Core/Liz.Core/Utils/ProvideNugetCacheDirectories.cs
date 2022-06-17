using Liz.Core.CliTool.Contracts;
using Liz.Core.Utils.Contracts;

namespace Liz.Core.Utils;

internal sealed class ProvideNugetCacheDirectories : IProvideNugetCacheDirectories
{
    private readonly ICliToolExecutor _cliToolExecutor;

    private IEnumerable<string>? _nugetCacheDirectories;

    public ProvideNugetCacheDirectories(
        ICliToolExecutor cliToolExecutor)
    {
        _cliToolExecutor = cliToolExecutor ?? throw new ArgumentNullException(nameof(cliToolExecutor));
    }

    public async Task<IEnumerable<string>> GetAsync()
    {
        _nugetCacheDirectories ??= await GetCacheDirectoriesAsync().ConfigureAwait(false);
        return _nugetCacheDirectories;
    }

    private async Task<IEnumerable<string>> GetCacheDirectoriesAsync()
    {
        // refer to https://docs.microsoft.com/en-us/dotnet/core/tools/dotnet-nuget-locals
        var result = await _cliToolExecutor
            .ExecuteWithResultAsync("dotnet", "nuget locals global-packages --list --force-english-output")
            .ConfigureAwait(false);
        var cacheDirectories = ParseCliOutput(result);

        return cacheDirectories;
    }

    private static IEnumerable<string> ParseCliOutput(string cliOutput)
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
