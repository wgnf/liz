using ArrangeContext.Moq;
using FluentAssertions;
using Liz.Core.Logging.Contracts;
using Liz.Tool.CommandLine;
using Xunit;

namespace Liz.Tool.Tests.CommandLine;

public sealed class CommandProviderTests
{
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
    public void Provided_Root_Command_Should_Have_File_Input_Argument()
    {
        var sut = new ArrangeContext<CommandProvider>().Build();

        var rootCommand = sut.Get();

        var argument = rootCommand.Arguments.FirstOrDefault(opt => opt.Name == "targetFile");
        Assert.NotNull(argument);

        argument?
            .Description
            .Should()
            .NotBeNullOrWhiteSpace();

        argument?
            .ValueType
            .Should()
            .Be<FileInfo>();
    }

    [Fact]
    public void Provided_Root_Command_Should_Have_Log_Level_Option()
    {
        var sut = new ArrangeContext<CommandProvider>().Build();

        var rootCommand = sut.Get();

        var option = rootCommand.Options.FirstOrDefault(opt => opt.Name == "log-level");
        Assert.NotNull(option);

        option?
            .Description
            .Should()
            .NotBeNullOrWhiteSpace();

        option?
            .ValueType
            .Should()
            .Be<LogLevel>();

        option?
            .Aliases
            .Should()
            .Contain(new[] { "--log-level", "-l" });
    }
    
    [Fact]
    public void Provided_Root_Command_Should_Have_Include_Transitive_Option()
    {
        var sut = new ArrangeContext<CommandProvider>().Build();

        var rootCommand = sut.Get();

        var option = rootCommand.Options.FirstOrDefault(opt => opt.Name == "include-transitive");
        Assert.NotNull(option);

        option?
            .Description
            .Should()
            .NotBeNullOrWhiteSpace();

        option?
            .ValueType
            .Should()
            .Be<bool>();

        option?
            .Aliases
            .Should()
            .Contain(new[] { "--include-transitive", "-i" });
    }
    
    [Fact]
    public void Provided_Root_Command_Should_Have_Suppress_Print_Details_Option()
    {
        var sut = new ArrangeContext<CommandProvider>().Build();

        var rootCommand = sut.Get();

        var option = rootCommand.Options.FirstOrDefault(opt => opt.Name == "suppress-print-details");
        Assert.NotNull(option);

        option?
            .Description
            .Should()
            .NotBeNullOrWhiteSpace();

        option?
            .ValueType
            .Should()
            .Be<bool>();

        option?
            .Aliases
            .Should()
            .Contain(new[] { "--suppress-print-details", "-sd" });
    }
    
    [Fact]
    public void Provided_Root_Command_Should_Have_Suppress_Print_Issues_Option()
    {
        var sut = new ArrangeContext<CommandProvider>().Build();

        var rootCommand = sut.Get();

        var option = rootCommand.Options.FirstOrDefault(opt => opt.Name == "suppress-print-issues");
        Assert.NotNull(option);

        option?
            .Description
            .Should()
            .NotBeNullOrWhiteSpace();

        option?
            .ValueType
            .Should()
            .Be<bool>();

        option?
            .Aliases
            .Should()
            .Contain(new[] { "--suppress-print-issues", "-si" });
    }
    
    [Fact]
    public void Provided_Root_Command_Should_Have_Suppress_Progressbar_Option()
    {
        var sut = new ArrangeContext<CommandProvider>().Build();

        var rootCommand = sut.Get();

        var option = rootCommand.Options.FirstOrDefault(opt => opt.Name == "suppress-progressbar");
        Assert.NotNull(option);

        option?
            .Description
            .Should()
            .NotBeNullOrWhiteSpace();

        option?
            .ValueType
            .Should()
            .Be<bool>();

        option?
            .Aliases
            .Should()
            .Contain(new[] { "--suppress-progressbar", "-sb" });
    }
    
    [Fact]
    public void Provided_Root_Command_Should_Have_License_Type_Definitions_Option()
    {
        var sut = new ArrangeContext<CommandProvider>().Build();

        var rootCommand = sut.Get();

        var option = rootCommand.Options.FirstOrDefault(opt => opt.Name == "license-type-definitions");
        Assert.NotNull(option);

        option?
            .Description
            .Should()
            .NotBeNullOrWhiteSpace();

        option?
            .ValueType
            .Should()
            .Be<string>();

        option?
            .Aliases
            .Should()
            .Contain(new[] { "--license-type-definitions", "-td" });
    }
    
    [Fact]
    public void Provided_Root_Command_Should_Have_Url_To_License_Type_Mapping_Option()
    {
        var sut = new ArrangeContext<CommandProvider>().Build();

        var rootCommand = sut.Get();

        var option = rootCommand.Options.FirstOrDefault(opt => opt.Name == "url-type-mapping");
        Assert.NotNull(option);

        option?
            .Description
            .Should()
            .NotBeNullOrWhiteSpace();

        option?
            .ValueType
            .Should()
            .Be<string>();

        option?
            .Aliases
            .Should()
            .Contain(new[] { "--url-type-mapping", "-um" });
    }
    
    [Fact]
    public void Provided_Root_Command_Should_Have_License_Type_Whitelist_Option()
    {
        var sut = new ArrangeContext<CommandProvider>().Build();

        var rootCommand = sut.Get();

        var option = rootCommand.Options.FirstOrDefault(opt => opt.Name == "whitelist");
        Assert.NotNull(option);

        option?
            .Description
            .Should()
            .NotBeNullOrWhiteSpace();

        option?
            .ValueType
            .Should()
            .Be<string>();

        option?
            .Aliases
            .Should()
            .Contain(new[] { "--whitelist", "-w" });
    }
    
    [Fact]
    public void Provided_Root_Command_Should_Have_License_Type_Blacklist_Option()
    {
        var sut = new ArrangeContext<CommandProvider>().Build();

        var rootCommand = sut.Get();

        var option = rootCommand.Options.FirstOrDefault(opt => opt.Name == "blacklist");
        Assert.NotNull(option);

        option?
            .Description
            .Should()
            .NotBeNullOrWhiteSpace();

        option?
            .ValueType
            .Should()
            .Be<string>();

        option?
            .Aliases
            .Should()
            .Contain(new[] { "--blacklist", "-b" });
    }
}
