using Cake.Core;
using Liz.Core.Logging.Contracts;
using System.Diagnostics.CodeAnalysis;

namespace Liz.Cake.Logging;

[ExcludeFromCodeCoverage] // simple provider factory
internal sealed class CakeLoggerProvider : ILoggerProvider
{
    private readonly ICakeContext _cakeContext;

    public CakeLoggerProvider(ICakeContext cakeContext)
    {
        _cakeContext = cakeContext ?? throw new ArgumentNullException(nameof(cakeContext));
    }
    
    public ILogger Get(LogLevel logLevel)
    {
        return new CakeLogger(_cakeContext);
    }
}
