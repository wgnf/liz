using FluentAssertions;
using Liz.Core.Settings;
using Xunit;

// ReSharper disable StringLiteralTypo

namespace Liz.Core.IntegrationTests;

public class LicenseTypesTests
{
    [Theory]
    [InlineData("xUnit", "2.4.1", "net6.0", new[] { "MIT", "Apache-2.0" })]
    [InlineData("OpenMcdf", "2.2.1.9 ", "net6.0", new[] { "MPL-2.0" })]
    public async Task Determines_Correct_License_Types(
        string packageName,
        string packageVersion,
        string targetFramework,
        IEnumerable<string> expectedTypes)
    {
        var projectFile = PrepareCsprojFile(packageName, targetFramework, packageVersion);

        var factory = new ExtractLicensesFactory();

        var settings = new TestExtractSettings(projectFile);
        var extract = factory.Create(settings);

        var result = await extract.ExtractAsync();

        var targetPackage = result.FirstOrDefault(package => package.Name == packageName);
        targetPackage
            .Should()
            .NotBeNull();

        targetPackage?
            .LicenseInformation
            .Types
            .Should()
            .BeEquivalentTo(expectedTypes);
    }

    private static string PrepareCsprojFile(
        string packageName,
        string targetFramework, 
        string packageVersion)
    {
        var temporaryDirectory = Path.GetTempPath();
        var temporaryProjectFile = Path.Combine(temporaryDirectory, "TestProject.csproj");

        var projectFileContent = @$"
<Project Sdk=""Microsoft.NET.Sdk"">
  <PropertyGroup>
    <TargetFramework>{targetFramework}</TargetFramework>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include=""{packageName}"" Version=""{packageVersion}"" />
  </ItemGroup>
</Project>";
        
        File.WriteAllText(temporaryProjectFile, projectFileContent);
        
        return temporaryProjectFile;
    }

    private class TestExtractSettings : ExtractLicensesSettingsBase
    {
        private readonly string _targetFile;

        public TestExtractSettings(string targetFile)
        {
            _targetFile = targetFile;
        }
        
        public override string GetTargetFile()
        {
            return _targetFile;
        }
    }
}
