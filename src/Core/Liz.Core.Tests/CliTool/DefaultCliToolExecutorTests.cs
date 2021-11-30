using ArrangeContext.Moq;
using DotnetNugetLicenses.Core.CliTool;
using FluentAssertions;
using System;
using System.Threading.Tasks;
using Xunit;

namespace DotnetNugetLicenses.Core.Tests.CliTool;

public class DefaultCliToolExecutorTests
{
    [Fact]
    public async Task Should_Fail_Execute_On_Invalid_Parameters()
    {
        var context = new ArrangeContext<DefaultCliToolExecutor>();
        var sut = context.Build();

        await Assert.ThrowsAsync<ArgumentException>(() => sut.ExecuteAsync(null!, "something"));
        await Assert.ThrowsAsync<ArgumentException>(() => sut.ExecuteAsync(string.Empty, "something"));
        await Assert.ThrowsAsync<ArgumentException>(() => sut.ExecuteAsync(" ", "something"));

        await Assert.ThrowsAsync<ArgumentNullException>(() => sut.ExecuteAsync("something", null!));
    }
    
    [Fact]
    public async Task Should_Be_Able_To_Execute_Wrong_Command_And_Throw()
    {
        var context = new ArrangeContext<DefaultCliToolExecutor>();
        var sut = context.Build();

        // i hope no one ever has this somewhere on his/her computer lol
        await Assert.ThrowsAsync<CliToolExecutionFailedException>(() => sut.ExecuteAsync("foo", "--bar"));
    }

    [Fact]
    public async Task Should_Be_Able_To_Execute_Command_With_Wrong_Argument_And_Throw()
    {
        var context = new ArrangeContext<DefaultCliToolExecutor>();
        var sut = context.Build();

        await Assert.ThrowsAsync<CliToolExecutionFailedException>(() => sut.ExecuteAsync("dotnet", "--bar"));
    }

    [Fact]
    public async Task Should_Be_Able_To_Execute_Dotnet_Version()
    {
        var context = new ArrangeContext<DefaultCliToolExecutor>();
        var sut = context.Build();

        await sut.ExecuteAsync("dotnet", "--version");
    }
    
    [Fact]
    public async Task Should_Fail_ExecuteWithResult_On_Invalid_Parameters()
    {
        var context = new ArrangeContext<DefaultCliToolExecutor>();
        var sut = context.Build();

        await Assert.ThrowsAsync<ArgumentException>(() => sut.ExecuteWithResultAsync(null!, "something"));
        await Assert.ThrowsAsync<ArgumentException>(() => sut.ExecuteWithResultAsync(string.Empty, "something"));
        await Assert.ThrowsAsync<ArgumentException>(() => sut.ExecuteWithResultAsync(" ", "something"));

        await Assert.ThrowsAsync<ArgumentNullException>(() => sut.ExecuteWithResultAsync("something", null!));
    }
    
    [Fact]
    public async Task Should_Be_Able_To_ExecuteWithResult_Dotnet_Version_And_Provide_Output()
    {
        var context = new ArrangeContext<DefaultCliToolExecutor>();
        var sut = context.Build();

        var result = await sut.ExecuteWithResultAsync("dotnet", "--version");

        result
            .Should()
            .NotBeNullOrWhiteSpace();
    }
}
