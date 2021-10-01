using ArrangeContext.Moq;
using DotnetNugetLicenses.Core.Contracts;
using Moq;
using System;
using System.IO;
using System.IO.Abstractions;
using Xunit;

namespace DotnetNugetLicenses.Core.Tests
{
    public class ExtractLicensesTests
    {
        [Fact]
        public void Should_Check_Provided_Target_File_If_Exists_And_Throw_When_Not()
        {
            var context = new ArrangeContext<ExtractLicenses>();
            var sut = context.Build();

            var targetFileMock = new Mock<IFileInfo>();
            targetFileMock
                .Setup(fileInfo => fileInfo.Exists)
                .Returns(false);

            var settings = new ExtractSettings(targetFileMock.Object);

            Assert.Throws<FileNotFoundException>(() => sut.Extract(settings));
        }

        [Theory]
        [InlineData("txt")]
        [InlineData("exe")]
        [InlineData("something super complicated")]
        public void Should_Check_Provided_Target_File_If_Correct_Extension_And_Throw_When_Not(string extension)
        {
            var context = new ArrangeContext<ExtractLicenses>();
            var sut = context.Build();

            var targetFileMock = new Mock<IFileInfo>();
            targetFileMock
                .Setup(fileInfo => fileInfo.Exists)
                .Returns(true);
            targetFileMock
                .Setup(fileInfo => fileInfo.Extension)
                .Returns(extension);

            var settings = new ExtractSettings(targetFileMock.Object);

            Assert.Throws<ArgumentException>(() => sut.Extract(settings));
        }
    }
}
