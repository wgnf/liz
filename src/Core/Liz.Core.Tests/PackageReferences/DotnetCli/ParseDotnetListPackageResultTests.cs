using ArrangeContext.Moq;
using FluentAssertions;
using Liz.Core.PackageReferences.Contracts.Models;
using Liz.Core.PackageReferences.DotnetCli;
using Xunit;

namespace Liz.Core.Tests.PackageReferences.DotnetCli;

public class ParseDotnetListPackageResultTests
{
    [Fact]
    public void Parse_Should_Throw_On_Invalid_Parameters()
    {
        var context = new ArrangeContext<ParseDotnetListPackageResult>();
        var sut = context.Build();

        Assert.Throws<ArgumentNullException>(() => sut.Parse(null!));
    }
    
    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public void Parse_Should_Be_Robust_Against_Empty_Input(string input)
    {
        var context = new ArrangeContext<ParseDotnetListPackageResult>();
        var sut = context.Build();

        var result = sut.Parse(input);

        result
            .Should()
            .NotBeNull()
            .And
            .BeEmpty();
    }
    
    [Fact]
    public void Should_Be_Able_To_Parse_Example_Output()
    {
        var context = new ArrangeContext<ParseDotnetListPackageResult>();
        var sut = context.Build();

        const string exampleOutput = @"Project 'SentimentAnalysis' has the following package references
    [netcoreapp2.1]:
    Top-level Package               Requested   Resolved
    > Microsoft.ML                  1.4.0       1.4.0
    > Microsoft.NETCore.App   (A)   [2.1.0, )   2.1.0
    
    (A) : Auto-referenced package.";

        var expectedReferences = new List<PackageReference>
        {
            new("Microsoft.ML", "netcoreapp2.1", "1.4.0"), 
            new("Microsoft.NETCore.App", "netcoreapp2.1", "2.1.0")
        };
        
        var packageReferences = sut.Parse(exampleOutput);

        packageReferences
            .Should()
            .BeEquivalentTo(expectedReferences);
    }

    [Fact]
    public void Should_Be_Able_To_Parse_Output_With_Multiple_Target_Frameworks()
    {
        var context = new ArrangeContext<ParseDotnetListPackageResult>();
        var sut = context.Build();
        
        const string output = @"Project 'Liz.Core' has the following package references
   [net5.0]:
   Top-level Package             Requested   Resolved
   > JetBrains.Annotations       2021.3.0    2021.3.0
   > SlnParser                   2.0.0       2.0.0
   > System.IO.Abstractions      13.2.47     13.2.47

   Transitive Package                        Resolved
   > Microsoft.NETCore.Platforms             5.0.0
   > System.IO.FileSystem.AccessControl      5.0.0
   > System.Security.AccessControl           5.0.0
   > System.Security.Principal.Windows       5.0.0

   [net6.0]:
   Top-level Package             Requested   Resolved
   > JetBrains.Annotations       2021.3.0    2021.3.0
   > SlnParser                   2.0.0       2.0.0
   > System.IO.Abstractions      13.2.47     13.2.47

   Transitive Package                        Resolved
   > Microsoft.NETCore.Platforms             5.0.0
   > System.IO.FileSystem.AccessControl      5.0.0
   > System.Security.AccessControl           5.0.0
   > System.Security.Principal.Windows       5.0.0";
        
        var expectedReferences = new List<PackageReference>
        {
            new("JetBrains.Annotations", "net5.0", "2021.3.0"),
            new("SlnParser", "net5.0", "2.0.0"),
            new("System.IO.Abstractions", "net5.0", "13.2.47"),
            new("Microsoft.NETCore.Platforms", "net5.0", "5.0.0"),
            new("System.IO.FileSystem.AccessControl", "net5.0", "5.0.0"),
            new("System.Security.AccessControl", "net5.0", "5.0.0"),
            new("System.Security.Principal.Windows", "net5.0", "5.0.0"),
            
            new("JetBrains.Annotations", "net6.0", "2021.3.0"),
            new("SlnParser", "net6.0", "2.0.0"),
            new("System.IO.Abstractions", "net6.0", "13.2.47"),
            new("Microsoft.NETCore.Platforms", "net6.0", "5.0.0"),
            new("System.IO.FileSystem.AccessControl", "net6.0", "5.0.0"),
            new("System.Security.AccessControl", "net6.0", "5.0.0"),
            new("System.Security.Principal.Windows", "net6.0", "5.0.0")
        };
        
        var packageReferences = sut.Parse(output);

        packageReferences
            .Should()
            .BeEquivalentTo(expectedReferences);
    }
    
    [Fact]
    // c.f.: https://github.com/wgnf/liz/issues/32
    public void Should_Be_Able_To_Parse_Example_Output_With_A_Non_English_Output()
    {
        var context = new ArrangeContext<ParseDotnetListPackageResult>();
        var sut = context.Build();

        // ReSharper disable StringLiteralTypo
        const string exampleOutput = @"Das Projekt 'SentimentAnalysis' enthält die folgenden Paketverweise.
    [netcoreapp2.1]:
    Paket oberster Ebene               Angefordert   Aufgelöst
    > Microsoft.ML                     1.4.0         1.4.0
    > Microsoft.NETCore.App   (A)      [2.1.0, )     2.1.0
    
    (A) : Auto-referenced package.";
        // ReSharper restore StringLiteralTypo

        var expectedReferences = new List<PackageReference>
        {
            new("Microsoft.ML", "netcoreapp2.1", "1.4.0"), 
            new("Microsoft.NETCore.App", "netcoreapp2.1", "2.1.0")
        };
        
        var packageReferences = sut.Parse(exampleOutput);

        packageReferences
            .Should()
            .BeEquivalentTo(expectedReferences);
    }
}
