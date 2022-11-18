using ArrangeContext.Moq;
using FluentAssertions;
using Liz.Core.CliTool.Contracts;
using Liz.Core.Utils;
using Moq;
using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;
using Xunit;

namespace Liz.Core.Tests.Utils;

public class ProvideNugetCacheDirectoriesTests
{
    [Fact]
    public async Task Provides_Cache_Directories()
    {
        var context = ArrangeContext<ProvideNugetCacheDirectories>.Create();
        context.Use<IFileSystem>(new MockFileSystem());
        
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
            .Contain(directory => directory == expectedDirectory);

        // getting the directory should be cached, so a second call should just return the cache
        _ = await sut.GetAsync();
        
        context
            .For<ICliToolExecutor>()
            .Verify(cliToolExecutor => cliToolExecutor.ExecuteWithResultAsync(It.IsAny<string>(), It.IsAny<string>()),
                Times.Once);
    }
}
