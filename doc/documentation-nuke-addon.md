# Liz.Nuke

`Liz.Nuke` is the Nuke-Addon for **liz** which allows you to analyze your projects or solutions using the convenience of Nuke i.e. in your CI/CD pipelines.

## Installation

Because Nuke basically is just a console-application the normal process of adding a NuGet-Package applies

```bash
# using the Package Manager
> Install-Package Liz.Nuke

# using the .NET CLI
> dotnet add package Liz.Nuke
```

## Usage

The Addin adds a new static Tasks-Class containing the following method:

```cs
Task<IEnumerable<PackageReference>> ExtractLicensesAsync(Configure<ExtractLicensesSettings> configureSettings)
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

To support the Nuke-specific way of configuring the settings in a Fluent-API way, following Extensions were added as well:

| Name | Description |
|------|-------------|
| `SetTargetFile` | Sets the `TargetFile` Property to the given value |
| | |
| `SetIncludeTransitiveDependencies` | Sets the `IncludeTransitiveDependencies` Property to the given value |
| `EnableIncludeTransitiveDependencies` | Sets the `IncludeTransitiveDependencies` Property to `true` |
| `DisableIncludeTransitiveDependencies` | Sets the `IncludeTransitiveDependencies` Property to `false` |
| `ToggleIncludeTransitiveDependencies` | Toggles the `IncludeTransitiveDependencies` Property |
| `ResetIncludeTransitiveDependencies` | Sets the `IncludeTransitiveDependencies` Property to default |
| | |
| `SetSuppressPrintDetails` | Sets the `SuppressPrintDetails` Property to the given value |
| `EnableSuppressPrintDetails` | Sets the `SuppressPrintDetails` Property to `true` |
| `DisableSuppressPrintDetails` | Sets the `SuppressPrintDetails` Property to `false` |
| `ToggleSuppressPrintDetails` | Toggles the `SuppressPrintDetails` Property |
| `ResetSuppressPrintDetails` | Sets the `SuppressPrintDetails` Property to default |
| | |
| `SetSuppressPrintIssues` | Sets the `SuppressPrintIssues` Property to the given value |
| `EnableSuppressPrintIssues` | Sets the `SuppressPrintIssues` Property to `true` |
| `DisableSuppressPrintIssues` | Sets the `SuppressPrintIssues` Property to `false` |
| `ToggleSuppressPrintIssues` | Toggles the `SuppressPrintIssues` Property |
| `ResetSuppressPrintIssues` | Sets the `SuppressPrintIssues` Property to default |

## Example Usage

```cs
Target ExtractLicenses => _ => _
  .Executes(async () =>
  {
    var targetFile = (AbsolutePath)"path/to/project.csproj";
    
    await ExtractLicensesTasks.ExtractLicensesAsync(settings => settings
      .SetTargetFile(targetFile)
      .EnableIncludeTransitiveDependencies());
  });
```