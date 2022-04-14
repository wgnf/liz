using ArrangeContext.Moq;
using FluentAssertions;
using Liz.Core.Utils;
using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;
using Xunit;

namespace Liz.Core.Tests.Utils;

public class FileContentProviderTests
{
    [Fact]
    public async Task Fails_On_Invalid_Parameters()
    {
        var context = ArrangeContext<FileContentProvider>.Create();
        var sut = context.Build();

        await Assert.ThrowsAsync<ArgumentException>(() => sut.GetFileContentAsync(null!));
        await Assert.ThrowsAsync<ArgumentException>(() => sut.GetFileContentAsync(""));
        await Assert.ThrowsAsync<ArgumentException>(() => sut.GetFileContentAsync(" "));
    }

    [Fact]
    public async Task Gets_Content_From_Local_File_With_Absolute_Path()
    {
        const string fileContent = "123";

        var mockFileSystem = new MockFileSystem();

        var path = mockFileSystem.Path.GetTempFileName();
        mockFileSystem.AddFile(path, new MockFileData(fileContent));
        
        var context = ArrangeContext<FileContentProvider>.Create();
        context.Use<IFileSystem>(mockFileSystem);
        
        var sut = context.Build();

        var result = await sut.GetFileContentAsync(path);

        result
            .Should()
            .Be(fileContent);
    }
    
    [Fact]
    public async Task Gets_Content_From_Local_File_With_Relative_Path()
    {
        const string path = "../../something.txt";
        const string fileContent = "123";

        var mockFileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
        {
            { path, new MockFileData(fileContent) }
        });
        
        var context = ArrangeContext<FileContentProvider>.Create();
        context.Use<IFileSystem>(mockFileSystem);
        
        var sut = context.Build();

        var result = await sut.GetFileContentAsync(path);

        result
            .Should()
            .Be(fileContent);
    }
}
