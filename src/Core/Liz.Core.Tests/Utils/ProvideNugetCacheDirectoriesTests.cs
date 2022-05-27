using ArrangeContext.Moq;
using FluentAssertions;
using Liz.Core.CliTool.Contracts;
using Liz.Core.Utils;
using Moq;
using Xunit;

namespace Liz.Core.Tests.Utils;

public class ProvideNugetCacheDirectoriesTests
{
    [Fact]
    public async Task Provides_Cache_Directories()
    {
        var context = ArrangeContext<ProvideNugetCacheDirectories>.Create();
        var sut = context.Build();

        const string expectedDirectory = @"C:\Users\some-user\.nuget\packages\";
        const string exampleResult = $@"global-packages: {expectedDirectory}";
        context
            .For<ICliToolExecutor>()
            .Setup(cliToolExecutor => cliToolExecutor.ExecuteWithResultAsync(It.IsAny<string>(), It.IsAny<string>()))
            .Returns(Task.FromResult(exampleResult));

        var result = await sut.GetAsync();

        result
            .Should()
            .OnlyContain(directory => directory == expectedDirectory);

        // getting the directory should be cached, so a second call should just return the cache
        _ = await sut.GetAsync();
        
        context
            .For<ICliToolExecutor>()
            .Verify(cliToolExecutor => cliToolExecutor.ExecuteWithResultAsync(It.IsAny<string>(), It.IsAny<string>()),
                Times.Once);
    }
}
