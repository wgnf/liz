using DotnetNugetLicenses.Tool.Contracts.CommandLine;
using FluentAssertions;
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
    }
}
