using DotnetNugetLicenses.Tool.Contracts.CommandLine;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using System;
using Unity;
using Xunit;

namespace DotnetNugetLicenses.Tool.Tests
{
    public sealed class ToolRegistrationExtensionsTests
    {
        [Theory]
        [InlineData(typeof(ICommandProvider))]
        [InlineData(typeof(ICommandRunner))]
        public void Should_Register_Needed_CommandLine_Tool_Services(Type typeToCheck)
        {
            using var container = new UnityContainer();

            container.RegisterToolServices();

            container
                .IsRegistered(typeToCheck)
                .Should()
                .BeTrue();
        }

        [Theory]
        [InlineData(typeof(ILoggerFactory))]
        [InlineData(typeof(ILogger<>))]
        public void Should_Register_Needed_Logging_Services(Type typeToCheck)
        {
            using var container = new UnityContainer();

            container.RegisterLoggingServices(LogLevel.Information);

            container
                .IsRegistered(typeToCheck)
                .Should()
                .BeTrue();
        }

        [Fact]
        public void Container_Should_Be_Able_To_Create_Scoped_ILogger_Instance()
        {
            using var container = new UnityContainer();
            
            container.RegisterLoggingServices(LogLevel.Information);

            var logger = container.Resolve<ILogger<ToolRegistrationExtensionsTests>>();
            logger
                .Should()
                .NotBeNull();
        }
    }
}
