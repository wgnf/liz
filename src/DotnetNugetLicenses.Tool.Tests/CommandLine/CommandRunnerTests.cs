using ArrangeContext.Moq;
using DotnetNugetLicenses.Core.Contracts;
using DotnetNugetLicenses.Tool.CommandLine;
using DotnetNugetLicenses.Tool.Contracts.CommandLine;
using FluentAssertions;
using Moq;
using System;
using System.IO;
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

			var fileInfo = new FileInfo("some/file.txt");

			sut.Run(fileInfo);

			context
				.For<IExtractLicenses>()
				.Verify(e => e.Extract(It.Is<ExtractSettings>(settings =>
					settings.TargetFile.FullName == fileInfo.FullName)), Times.Once());
		}
	}
}
