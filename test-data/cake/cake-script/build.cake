#addin nuget:?package=Cake.ExtractLicenses&prerelease

var target = Argument("target", "Default");
var targetFile = File("../../sln/TestingGround.sln");

Task("Default")
    .Does(() =>
    {
        Information("Hey... You've not specified any task to run!");
    });

Task("ExtractLicensesDefault")
    .Does(async () => 
    {
        await ExtractLicensesAsync(targetFile);
    });

Task("ExtractLicenses")
    .Does(async () => 
    {
        var settings = new ExtractLicensesToolSettings
        {
            IncludeTransitiveDependencies = true
        };

        await ExtractLicensesAsync(targetFile, settings);
    });

RunTarget(target);
