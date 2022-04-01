# Cake.ExtractLicenses

`Cake.ExtractLicenses` is the Cake-Addin for **liz** which allows you to analyze your projects or solutions using the convenience of Cake i.e. in your CI/CD pipelines.

## Installation

To install the Addin you have to do one of the following, depending on how you use Cake

### Cake-Script

Add the following directive to your `.cake` script

```cs
#addin nuget:?package=Cake.ExtractLicenses&version=1.0.0&loaddependencies=true
```

:warning: **Note:**  
  
- It's always good to use a specific version of your Addin, so that no automatic upgrades happen, which can possibly break your script because of breaking changes
- The additional `&loaddependencies=true` is required, because the Addin depends on the `Liz.Core` package, but dependencies are usually not loaded by default when using Cake-Scripts

### Cake-Frosting

Because Cake-Frosting basically is just a console-application the normal process of adding a NuGet-Package applies

```bash
# using the Package Manager
> Install-Package Cake.ExtractLicenses

# using the .NET CLI
> dotnet add package Cake.ExtractLicenses
```

## Usage

The Addin adds a new Method-Alias to Cake:

```cs
Task<IEnumerable<PackageReference>> ExtractLicensesAsync(this ICakeContext context, ExtractLicensesSettings settings);
```

This method can be used to analyze your solution or project according to the provided settings.

### Settings

The settings contain the following properties which can be set according to your needs

| Name | Description |
|------|-------------|
| `TargetFile` | The target file that needs to be analyzed - can be a `.csproj`, `.fsproj` or `sln` file |
| `IncludeTransitiveDependencies` | Whether or not to include transitive (dependencies of dependencies) dependencies </br> Default: `false` |
| `SuppressPrintDetails` | Whether or not to suppress printing details of analyzed package-references and license-information </br> Default: `false` |
| `SuppressPrintIssues` | Whether or not to suppress printing found issues of analyzed package-references and license-information </br> Default: `false` |
| `LicenseTypeDefinitions` | A list of `LicenseTypeDefinition`s that describe license-types by providing inclusive/exclusive license-text snippets |

## Example Usages

### Cake-Script

```cs
Task("ExtractLicenses")
  .Does(async () => 
  {
    var targetFile = File("path/to/project.csproj");
    var settings = new ExtractLicensesToolSettings
    {
      TargetFile = targetFile,
      IncludeTransitiveDependencies = true
    };

    await ExtractLicensesAsync(settings);
  });
```

### Cake-Frosting

```cs
[TaskName("ExtractLicenses")]
public sealed class ExtractLicensesTask : AsyncFrostingTask<BuildContext>
{
  public override async Task RunAsync(BuildContext context)
  {
    var targetFile = context.File("path/to/project.csproj");
    var settings = new ExtractLicensesSettings
    {
        TargetFile = targetFile,
        IncludeTransitiveDependencies = true
    };
    await context.ExtractLicensesAsync(settings);
  }
}
```

### Adding your own license-type definitions

**liz** will try to guess license-types by their license-text when no license-type could be determined yet.
To cover a wide variety of license-types there are already lots of definitions added in the source by default.
But if you want to add a definition by yourself, you can do it, like so:

```cs

/*
 * this will add the license-type "LIZ-1.0" for every license-text that contains the string
 * "LIZ PUBLIC LICENSE 1.0"
 */
var definition1 = new LicenseTypeDefinition("LIZ-1.0", "LIZ PUBLIC LICENSE 1.0");

/*
 * a bit more advanced - the license-type "LIZ-2.0" will be added, when:
 * - the license-text contains "LIZ PUBLIC LICENSE" AND "v2.0"
 * - the license-text NOT contains "Version 1"
 */
var definition2 = new LicenseTypeDefinition("LIZ-2.0", "LIZ PUBLIC LICENSE", "v2.0")
{
  ExclusiveTextSnippets = new []{ "Version 1" }
};

var settings = new ExtractLicensesSettings
{
    LicenseTypeDefinitions = new List<LicenseTypeDefinition> { definition1, definition2 }
};

```
