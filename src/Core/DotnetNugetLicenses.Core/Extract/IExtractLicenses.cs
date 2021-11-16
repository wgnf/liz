using DotnetNugetLicenses.Core.Settings;
using System.Threading.Tasks;

namespace DotnetNugetLicenses.Core.Extract;

public interface IExtractLicenses
{
    Task ExtractAsync();
}
