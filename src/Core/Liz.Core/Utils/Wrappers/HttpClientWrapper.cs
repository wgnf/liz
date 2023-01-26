using Liz.Core.Settings;
using Liz.Core.Utils.Contracts.Wrappers;
using System.Diagnostics.CodeAnalysis;

namespace Liz.Core.Utils.Wrappers;

[ExcludeFromCodeCoverage] // wrapper for HttpClient
internal sealed class HttpClientWrapper : IHttpClient
{
    private readonly HttpClient _httpClient;
    
    public HttpClientWrapper(ExtractLicensesSettingsBase settings)
    {
        if (settings == null) throw new ArgumentNullException(nameof(settings));

        _httpClient = new HttpClient
        {
            Timeout = settings.RequestTimeout
        };
    }

    public Task<string> GetStringAsync(string url)
    {
        return _httpClient.GetStringAsync(url);
    }

    public Task<HttpResponseMessage> GetAsync(string url)
    {
        return _httpClient.GetAsync(url);
    }
}
