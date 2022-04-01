# Liz.Tool

`Liz.Tool` is the `dotnet`-CLI-Tool for **liz** which allows you to analyze your projects or solutions on your command-line.

## Installation

To install the tool you have to do one of the following

### Local Installation

```bash
# if you don't have a manifest file yet
> dotnet new tool-manifest

> dotnet tool install --local liz.tool

# run liz using
> dotnet liz --version
```

### Global Installation

```bash
> dotnet tool install --global liz.tool

# run liz using
> liz --version
```

## Usage

To get information about your currently installed version you can run

```bash
# global
> liz --version

# local
> dotnet liz --version
```

To get information about all options you can run

```bash
# global
> liz --help

# local
> dotnet liz --help
```

To analyze a project your solution you have to use

```bash
# global
> liz <targetFile> [options]

# local
> dotnet liz <targetFile> [options]
```

### Arguments

| Name | Description |
|------|-------------|
| `targetFile` | The target file to analyze. Can be a solution (`.sln`) or a project (`.csproj`, `.fsproj`) file |

### Options

| Name | Description |
|------|-------------|
| `--log-level`, `-l` | The log-level which describes what kind of messages are displayed when running the tool. </br> Possible values: `Trace`, `Debug`, `Information`, `Warning`, `Error`, `Critical`, `None` </br> Default: `Information` |
| `--include-transitive`, `-i` | If transitive dependencies should be included or not </br> Default: `false` |
| `--suppress-print-details`, `-sd` | If printing the license and package-reference details should be suppressed or not </br> Default: `false` |
| `--suppress-print-issues`, `-si` | If printing the license-information issues should be suppressed or not </br> Default: `false` |
| `--suppress-progressbar`, `-sb` | If displaying the progressbar should be suppressed or not. </br> Can help when debugging errors or is used in a CI/CD Pipeline </br> Default: `false` |
| `--license-type-definitions`, `-td` | Provide a path to a JSON-File providing license-type-definitions which describe license-types by providing inclusive/exclusive license-text snippets |

## Examples

### Simply running **liz** on a solution

```bash
# global
> liz "path/to/solution.sln"

# local
> dotnet liz "path/to/solution.sln"
```

### And on a project

```bash
# global
> liz "path/to/project.csproj"

# local
> dotnet liz "path/to/project.csproj"
```

Running **liz** on a project including transitive dependencies

```bash
# global
> liz "path/to/project.csproj" --include-transitive

# local
> dotnet liz "path/to/project.csproj" --include-transitive
```

### Adding your own license-type definitions

**liz** will try to guess license-types by their license-text when no license-type could be determined yet.
To cover a wide variety of license-types there are already lots of definitions added in the source by default.
But if you want to add a definition by yourself, you can do it, like so:  
  
Create a JSON-file that contains your definitions - `definitions.json` in this case:

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

This will do the following:

- definition "LIZ-1.0", will be added when:
  - the license-text contains the string "LIZ PUBLIC LICENSE 1.0"
- definition "LIZ-2.0", will be added when:
  - the license-text contains "LIZ PUBLIC LICENSE" AND "v2.0"
  - the license-text NOT contains "Version 1"

```bash
# global
> liz "path/to/project.csproj" --license-type-definitions "path/to/definitions.json"

# local
> dotnet liz "path/to/project.csproj" --license-type-definitions "path/to/definitions.json"
```
