﻿using ArrangeContext.Moq;
using DotnetNugetLicenses.Core.Logging;
using DotnetNugetLicenses.Tool.CommandLine;
using DotnetNugetLicenses.Tool.Contracts.CommandLine;
using FluentAssertions;
using System.IO;
using System.Linq;
using Xunit;

namespace DotnetNugetLicenses.Tool.Tests.CommandLine;

public sealed class CommandProviderTests
{
    [Fact]
    public void Should_Have_Correct_Interface()
    {
        var sut = new ArrangeContext<CommandProvider>().Build();
        sut
            .Should()
            .BeAssignableTo<ICommandProvider>();
    }

    [Fact]
    public void Should_Provide_Root_Command()
    {
        var sut = new ArrangeContext<CommandProvider>().Build();

        var rootCommand = sut.Get();
        rootCommand
            .Should()
            .NotBeNull();
        rootCommand
            .Description
            .Should()
            .NotBeNullOrWhiteSpace();
    }

    [Fact]
    public void Provided_Root_Command_Should_Have_File_Input_Option()
    {
        var sut = new ArrangeContext<CommandProvider>().Build();

        var rootCommand = sut.Get();

        var inputOption = rootCommand.Options.FirstOrDefault(opt => opt.Name == "targetFile");
        Assert.NotNull(inputOption);

        inputOption
            .Description
            .Should()
            .NotBeNullOrWhiteSpace();

        inputOption
            .ValueType
            .Should()
            .Be<FileInfo>();

        inputOption
            .Aliases
            .Should()
            .Contain(alias =>
                alias == "--target" ||
                alias == "-t");
    }

    [Fact]
    public void Provided_Root_Command_Should_Have_Log_Level_Option()
    {
        var sut = new ArrangeContext<CommandProvider>().Build();

        var rootCommand = sut.Get();

        var logLevelOption = rootCommand.Options.FirstOrDefault(opt => opt.Name == "logLevel");
        Assert.NotNull(logLevelOption);

        logLevelOption
            .Description
            .Should()
            .NotBeNullOrWhiteSpace();

        logLevelOption
            .ValueType
            .Should()
            .Be<LogLevel>();

        logLevelOption
            .Aliases
            .Should()
            .Contain(alias =>
                alias == "--log-level" ||
                alias == "-l");
    }
}