using ArrangeContext.Moq;
using Liz.Core.CliTool;
using Xunit;

namespace Liz.Core.Tests.CliTool;

public class DefaultCliToolExecutorTests
{
    [Fact]
    public async Task Should_Fail_Execute_On_Invalid_Parameters()
    {
        var context = ArrangeContext<DefaultCliToolExecutor>.Create();
        var sut = context.Build();

        await Assert.ThrowsAsync<ArgumentException>(() => sut.ExecuteAsync(null!, "something"));
        await Assert.ThrowsAsync<ArgumentException>(() => sut.ExecuteAsync(string.Empty, "something"));
        await Assert.ThrowsAsync<ArgumentException>(() => sut.ExecuteAsync(" ", "something"));

        await Assert.ThrowsAsync<ArgumentNullException>(() => sut.ExecuteAsync("something", null!));
    }
    
    [Fact]
    public async Task Should_Fail_ExecuteWithResult_On_Invalid_Parameters()
    {
        var context = ArrangeContext<DefaultCliToolExecutor>.Create();
        var sut = context.Build();

        await Assert.ThrowsAsync<ArgumentException>(() => sut.ExecuteWithResultAsync(null!, "something"));
        await Assert.ThrowsAsync<ArgumentException>(() => sut.ExecuteWithResultAsync(string.Empty, "something"));
        await Assert.ThrowsAsync<ArgumentException>(() => sut.ExecuteWithResultAsync(" ", "something"));

        await Assert.ThrowsAsync<ArgumentNullException>(() => sut.ExecuteWithResultAsync("something", null!));
    }
}
