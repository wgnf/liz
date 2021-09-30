using DotnetNugetLicenses.Core.Contracts;
using FluentAssertions;
using System;
using System.IO.Abstractions;
using Unity;
using Xunit;

namespace DotnetNugetLicenses.Core.Tests
{
    public sealed class CoreRegistrationExtensionsTests
    {
        [Theory]
        [InlineData(typeof(IExtractLicenses))]
        public void Should_Register_Needed_Core_Services(Type typeToCheck)
        {
            using var container = new UnityContainer();

            container.RegisterCoreServices();

            container
                .IsRegistered(typeToCheck)
                .Should()
                .BeTrue();
        }
        
        [Theory]
        [InlineData(typeof(IFileSystem))]
        public void Should_Register_Other_Needed_Services(Type typeToCheck)
        {
            using var container = new UnityContainer();

            container.RegisterOtherServices();

            container
                .IsRegistered(typeToCheck)
                .Should()
                .BeTrue();
        }
    }
}
