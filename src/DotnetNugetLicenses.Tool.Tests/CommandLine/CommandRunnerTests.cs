using ArrangeContext.Moq;
using DotnetNugetLicenses.Core.Contracts;
using DotnetNugetLicenses.Tool.CommandLine;
using DotnetNugetLicenses.Tool.Contracts.CommandLine;
using FluentAssertions;
using Moq;
using System;
using System.IO.Abstractions;
using Xunit;

namespace DotnetNugetLicenses.Tool.Tests.CommandLine
{
	public class CommandRunnerTests
	{
		[Fact]
		public void Should_Have_Correct_Interface()
		{
			var sut = new ArrangeContext<CommandRunner>().Build();

			sut
				.Should()
				.BeAssignableTo<ICommandRunner>();
		}

		[Fact]
		public void Should_Fail_To_Run_When_Invalid_Parameters()
		{
			var sut = new ArrangeContext<CommandRunner>().Build();

			Assert.Throws<ArgumentNullException>(() => sut.Run(null));
		}

		[Fact]
		public void Should_Forward_Execution_To_Extract_Licenses_With_Settings()
		{
			var context = new ArrangeContext<CommandRunner>();
			var sut = context.Build();

			const string file = "some/file";

			var fileInfo = new Mock<IFileInfo>();
			context
				.For<IFileSystem>()
				.Setup(f => f.FileInfo.FromFileName(file))
				.Returns(fileInfo.Object);

			sut.Run(file);

			context
				.For<IExtractLicenses>()
				.Verify(e => e.Extract(It.Is<ExtractSettings>(settings =>
					settings.TargetFile == fileInfo.Object)), Times.Once());
		}
	}
}
