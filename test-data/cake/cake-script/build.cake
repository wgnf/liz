#addin nuget:?package=Cake.ExtractLicenses&prerelease&loaddependencies=true

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
        var settings = new ExtractLicensesToolSettings
        {
            TargetFile = targetFile
        };

        await ExtractLicensesAsync(settings);
    });

Task("ExtractLicenses")
    .Does(async () => 
    {
        var settings = new ExtractLicensesToolSettings
        {
            TargetFile = targetFile,
            IncludeTransitiveDependencies = true
        };

        await ExtractLicensesAsync(settings);
    });

RunTarget(target);
