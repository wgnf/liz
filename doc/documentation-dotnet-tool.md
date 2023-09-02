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
| `--license-type-definitions`, `-td` | Provide a path to a JSON-File (local or remote - remote will be downloaded automatically if available) providing license-type-definitions which describe license-types by providing inclusive/exclusive license-text snippets |
| `--url-type-mapping`, `-um` | Provide a path to a JSON-file (local or remote - remote will be downloaded automatically if available) containing a mapping from license-url (key) to license-type (value) for licenses whose license-type could not be determined |
| `--whitelist`, `-w` | Provide a path to a JSON-File (local or remote - remote will be downloaded automatically if available) containing a list of license-types, which are the only ones allowed, when validating the determined license-types. Any license-type which is not in the whitelist will cause the validation to fail. </br> `--whitelist` and `--blacklist` are mutually exclusive! |
| `--blacklist`, `-b` |  Provide a path to a JSON-File (local or remote - remote will be downloaded automatically if available) containing a list of license-types, which are the only ones disallowed, when validating the determined license-types. Any license-type that is the same as within that blacklist will cause the validation to fail. Any other license-type is allowed. </br> `--whitelist` and `--blacklist` are mutually exclusive! |
| `--export-texts`, `-et` | A path to a directory to where the determined license-texts will be exported. </br> Each license-text will be written to an individual file with the file-name being: `<package-name>-<package-version>.txt`. If the license-text is the content of a website, the contents will be written into an \".html\" file instead |
| `--export-json`, `-ej` | A path to a JSON-file to which the determined license- and package-information will be exported. All the information will be written to a single JSON-file. </br> If the file already exists it will be overwritten. |
| `--timeout`, `-t` | The timeout for a request (i.e. to get the license text from a website) in **seconds**. </br> After this amount of time a request will be considered as failed and aborted. </br> This defaults to 10 seconds |
| `--project-excludes`, `-pe` | A path to a JSON-File (local or remote - remote will be downloaded automatically if available) containing a list of glob-patterns to exclude certain projects. A project will be excluded when it matches at least one glob-pattern. The pattern will be matched against the absolute path of the project-file. All available patterns can be found [here](https://github.com/dazinator/DotNet.Glob/tree/3.1.3#patterns) |

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

You can also use files that are stored remotely. Just use the web link to the resource:

```bash
# global
> liz "path/to/project.csproj" --license-type-definitions "http://path/to/definitions.json"

# local
> dotnet liz "path/to/project.csproj" --license-type-definitions "http://path/to/definitions.json"
```

### Adding your own license-url to license-type mappings

**liz** will try to guess license-types by their license-url when no license-type could be determined yet.
To cover a wide variety of license-types there are already lots of mappings added (i.e. for `choosealicense.com` and `opensource.org`) in the source by default.
But if you want to add a mapping by yourself, you can do it, like so:  
  
Create a JSON-file that contains your mappings - `mappings.json` in this case:

```json
{
  "https://liz.com/license": "LIZ-1.0"
}
```

This will add the license-type "LIZ-1.0" to every package-reference which has the exact license-url "https://liz.com/license".

```bash
# global
> liz "path/to/project.csproj" --url-type-mapping "path/to/mappings.json"

# local
> dotnet liz "path/to/project.csproj" --url-type-mapping "path/to/mappings.json"
```

You can also use files that are stored remotely. Just use the web link to the resource:

```bash
# global
> liz "path/to/project.csproj" --url-type-mapping "http://path/to/mappings.json"

# local
> dotnet liz "path/to/project.csproj" --url-type-mapping "http://path/to/mappings.json"
```

### Validating license-types

**liz** will validate the license-types of the determined package-references for you, if you provide a whitelist or blacklist. :warning: The options for the whitelist and blacklist are mutually exclusive (they cannot be used together)!  
What is the difference between a whitelist and a blacklist?

- whitelist: any license-type that is **not** explicitly referenced in the whitelist is not allowed
- blacklist: any license-type that is explicitly referenced in the blacklist is not allowed

#### Using a whitelist

Create a JSON-File that contains your whitelisted license-types - `whitelist.json` in this case:

```json
[
  "MIT",
  "Unlicense"
]
```

This will specifically only allow "MIT" and "Unlicense" licenses.

```bash
# global
> liz "path/to/project.csproj" --whitelist "path/to/whitelist.json"

# local
> dotnet liz "path/to/project.csproj" --whitelist "path/to/whitelist.json"
```

You can also use files that are stored remotely. Just use the web link to the resource:

```bash
# global
> liz "path/to/project.csproj" --whitelist "http://path/to/whitelist.json"

# local
> dotnet liz "path/to/project.csproj" --whitelist "http://path/to/whitelist.json"
```

#### Using a  blacklist

Create a JSON-File that contains your blacklisted license-types - `blacklist.json` in this case:

```json
[
  "GPL-3.0"
]
```

This will specifically disallow "GPL-3.0" licenses.

```bash
# global
> liz "path/to/project.csproj" --blacklist "path/to/blacklist.json"

# local
> dotnet liz "path/to/project.csproj" --blacklist "path/to/blacklist.json"
```

You can also use files that are stored remotely. Just use the web link to the resource:

```bash
# global
> liz "path/to/project.csproj" --blacklist "http://path/to/blacklist.json"

# local
> dotnet liz "path/to/project.csproj" --blacklist "http://path/to/blacklist.json"
```

#### Excluding projects

Create a JSON-File that contains your glob-patterns. If you want to exclude all your test-projects when you're scanning a whole solution, create a `project-excludes.json` (you can choose any other name of course) like this:

```json
[
  "*/**/*Tests.csproj"
]
```

This will disallow every project whose file-name ends with `Tests.csproj`. You can then use it like this:

```bash
# global
> liz "path/to/solution.sln" --project-excludes "path/to/project-excludes.json"

# local
> dotnet liz "path/to/solution.sln" --project-excludes "path/to/project-excludes.json"
```

You can also use files that are stored remotely. Just usse the web link to the resource:

```bash
# global
> liz "path/to/solution.sln" --project-excludes "http://path/to/project-excludes.json"

# local
> dotnet liz "path/to/solution.sln" --project-excludes "http://path/to/project-excludes.json"
```
