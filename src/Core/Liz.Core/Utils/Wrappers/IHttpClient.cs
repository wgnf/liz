using System.Threading.Tasks;

namespace Liz.Core.Utils.Wrappers;

internal interface IHttpClient
{
    Task<string> GetStringAsync(string url);
}
