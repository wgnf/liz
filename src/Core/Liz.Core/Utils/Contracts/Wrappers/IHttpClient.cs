namespace Liz.Core.Utils.Contracts.Wrappers;

internal interface IHttpClient
{
    Task<string> GetStringAsync(string url);
    Task<HttpResponseMessage> GetAsync(string url);
}
