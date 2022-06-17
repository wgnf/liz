namespace Liz.Core.Utils.Contracts;

internal interface IProvideNugetCacheDirectories
{
    Task<IEnumerable<string>> GetAsync();
}
