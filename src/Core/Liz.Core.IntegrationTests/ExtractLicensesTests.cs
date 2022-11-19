using FluentAssertions;
using Liz.Core.Logging.Null;
using Liz.Core.Settings;
using Xunit;

namespace Liz.Core.IntegrationTests;

public sealed class ExtractLicensesTests
{
    [Fact] // c.f.: https://github.com/wgnf/liz/issues/116
    public async Task Extract_Licenses_From_Sdk_Style_Project_Which_Has_A_Reference_To_An_Old_Style_Project()
    {
        var factory = new ExtractLicensesFactory();
        // this test-project has a project-reference to an old-style-project
        // NOTE: This path might have to be adjusted for other developers
        var settings = new ExtractLicensesSettings("../../../../../../test-data/sln/SdkStyleProject/SdkStyleProject.csproj")
        {
            // this problem is only apparent when we're including transitive dependencies, which include projects too
            IncludeTransitiveDependencies = true
        };
        
        var sut = factory.Create(settings, new NullLoggerProvider());

        var packageReferences = await sut.ExtractAsync();

        packageReferences
            .Should()
            .Contain(packageReference =>
                packageReference.Name == "Microsoft.Bcl" ||
                packageReference.Name == "Microsoft.Bcl.Build");
    }

    private class ExtractLicensesSettings : ExtractLicensesSettingsBase
    {
        private readonly string _targetFile;

        public ExtractLicensesSettings(string targetFile)
        {
            _targetFile = targetFile;
        }
        
        public override string GetTargetFile()
        {
            return _targetFile;
        }
    }
}
