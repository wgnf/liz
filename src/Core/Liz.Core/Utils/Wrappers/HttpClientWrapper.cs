using System.Diagnostics.CodeAnalysis;
using System.Net.Http;
using System.Threading.Tasks;

namespace Liz.Core.Utils.Wrappers;

[ExcludeFromCodeCoverage] // wrapper for HttpClient
internal sealed class HttpClientWrapper : IHttpClient
{
    private readonly HttpClient _httpClient;
    
    public HttpClientWrapper()
    {
        _httpClient = new HttpClient();
    }

    public Task<string> GetStringAsync(string url)
    {
        return _httpClient.GetStringAsync(url);
    }
}
