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
| `LicenseTypeDefinitions` | A list of `LicenseTypeDefinition`s that describe license-types by providing inclusive/exclusive license-text snippets |
| `LicenseTypeDefinitionsFilePath` | A path to a JSON-file (local or remote - remote will be downloaded automatically if available) containing a list of `LicenseTypeDefinition`s that describe license-types by providing inclusive/exclusive license-text snippets </br> If both `LicenseTypeDefinitions` and `LicenseTypeDefinitionsFilePath` are given those two will be merged |
| `UrlToLicenseTypeMapping` | A mapping from license-url (key) to license-type (value) for licenses whose license-type could not be determined |
| `UrlToLicenseTypeMappingFilePath` | A path to a JSON-file (local or remote - remote will be downloaded automatically if available) containing a mapping from license-url (key) to license-type (value) for licenses whose license-type could not be determined </br> If both `UrlToLicenseTypeMapping` and `UrlToLicenseTypeMappingFilePath` are given, those two will be merged, ignoring any duplicate keys |

To support the Nuke-specific way of configuring the settings in a Fluent-API way, following extensions were added as well:

| Name | Description |
|------|-------------|
| `SetTargetFile` | Sets the `TargetFile` Property to the given value |
| | |
| `SetIncludeTransitiveDependencies` | Sets the `IncludeTransitiveDependencies` property to the given value |
| `EnableIncludeTransitiveDependencies` | Sets the `IncludeTransitiveDependencies` property to `true` |
| `DisableIncludeTransitiveDependencies` | Sets the `IncludeTransitiveDependencies` property to `false` |
| `ToggleIncludeTransitiveDependencies` | Toggles the `IncludeTransitiveDependencies` property |
| `ResetIncludeTransitiveDependencies` | Sets the `IncludeTransitiveDependencies` property to default |
| | |
| `SetSuppressPrintDetails` | Sets the `SuppressPrintDetails` property to the given value |
| `EnableSuppressPrintDetails` | Sets the `SuppressPrintDetails` property to `true` |
| `DisableSuppressPrintDetails` | Sets the `SuppressPrintDetails` property to `false` |
| `ToggleSuppressPrintDetails` | Toggles the `SuppressPrintDetails` property |
| `ResetSuppressPrintDetails` | Sets the `SuppressPrintDetails` property to default |
| | |
| `SetSuppressPrintIssues` | Sets the `SuppressPrintIssues` property to the given value |
| `EnableSuppressPrintIssues` | Sets the `SuppressPrintIssues` property to `true` |
| `DisableSuppressPrintIssues` | Sets the `SuppressPrintIssues` property to `false` |
| `ToggleSuppressPrintIssues` | Toggles the `SuppressPrintIssues` property |
| `ResetSuppressPrintIssues` | Sets the `SuppressPrintIssues` property to default |
| | |
| `SetLicenseTypeDefinitions` | Sets the `LicenseTypeDefinitions` property to the given value |
| `SetLicenseTypeDefinitionsFilePath` | Set the `SetLicenseTypeDefinitionsFilePath` property to the given value |
| | |
| `SetUrlToLicenseTypeMapping` | Sets the `UrlToLicenseTypeMapping` property to the given value |
| `SetUrlToLicenseTypeMappingFilePath` | Sets the `UrlToLicenseTypeMappingFilePath` property to the given value |

## Example Usages

### Simple execution

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

await ExtractLicensesTasks.ExtractLicensesAsync(settings => settings
      .SetLicenseTypeDefinitions(new List<LicenseTypeDefinition>{ definition1, definition2 });

```

You can also reference a JSON-file containing the license-type-definitions in the settings, like so:  
  
example JSON-file:

```json
[
  {
    "type": "LIZ-1.0",
    "inclusiveText": [ "LIZ PUBLIC LICENSE 1.0" ]
  },

  {
    "type": "LIZ-2.0",
    "inclusiveText": [ "LIZ PUBLIC LICENSE", "v2.0" ],
    "exlusiveText": [ "Version 1" ]
  }
]
```

example usage:

```cs
// path to a file
await ExtractLicensesTasks.ExtractLicensesAsync(settings => settings
      .SetLicenseTypeDefinitionsFilePath("path/to/file.json"));

// or even a path to a remote file
await ExtractLicensesTasks.ExtractLicensesAsync(settings => settings
      .SetLicenseTypeDefinitionsFilePath("http://path/to/file.json"));
```

### Adding your own license-url to license-type mappings

**liz** will try to guess license-types by their license-url when no license-type could be determined yet.
To cover a wide variety of license-types there are already lots of mappings added (i.e. for `choosealicense.com` and `opensource.org`) in the source by default.
But if you want to add a mapping by yourself, you can do it, like so:

```cs

/*
 * this will add the license-type "LIZ-1.0" for every license-url which is exact "https://liz.com/license"
 */
await ExtractLicensesTasks.ExtractLicensesAsync(settings => settings
  .SetUrlToLicenseTypeMapping(new Dictionary<string, string>{ { "https://liz.com/license", "LIZ-1.0" } });
```

You can also reference a JSON-file containing the mappings in the settings, like so:  
  
example JSON-file:

```json
{
  "https://liz.com/license": "LIZ-1.0"
}
```

example usage:

```cs
// path to a file
await ExtractLicensesTasks.ExtractLicensesAsync(settings => settings
      .SetUrlToLicenseTypeMappingFilePath("path/to/file.json"));

// or even a path to a remote file
await ExtractLicensesTasks.ExtractLicensesAsync(settings => settings
      .SetUrlToLicenseTypeMappingFilePath("http://path/to/file.json"));
```
