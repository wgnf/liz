![liz logo](res/liz-logo-150x.png)

[![GitHub license](https://img.shields.io/badge/License-MIT-blue.svg)](LICENSE)
[![Uses SemVer 2.0.0](https://img.shields.io/badge/Uses%20SemVer-2.0.0-green)](https://semver.org/spec/v2.0.0.html)
[![Latest Release](https://img.shields.io/github/v/release/wgnf/liz?label=latest%20release&sort=semver)](https://github.com/wgnf/liz/releases)
[![codecov](https://codecov.io/gh/wgnf/liz/branch/main/graph/badge.svg?token=NMGXDYZMFA)](https://codecov.io/gh/wgnf/liz)  
[![GitHub stars](https://img.shields.io/github/stars/wgnf/liz?style=social)](https://github.com/wgnf/liz/stargazers)
[![GitHub forks](https://img.shields.io/github/forks/wgnf/liz?style=social)](https://github.com/wgnf/liz/network/members)
[![GitHub watchers](https://img.shields.io/github/watchers/wgnf/liz?style=social)](https://github.com/wgnf/liz/watchers)

**liz** (ˈlɪz - like the nickname for a person named "Elizabeth") is a tool to extract license-information from your project/solution aimed on a **fast** and **correct** process. Whether it's via a dotnet-CLI-Tool, Cake-Addin or Nuke-Addon.

---

## 🖥️ Tools

| Tool | Documentation | Version | Downloads |
|------|---------------|---------|-----------|
| `Liz.Tool` | [link](doc/documentation-dotnet-tool.md) | [![Latest Release .NET Tool](https://img.shields.io/nuget/v/Liz.Tool)](https://www.nuget.org/packages/Liz.Tool/) | [![Downloads .NET Tool](https://img.shields.io/nuget/dt/Liz.Tool)](https://www.nuget.org/packages/Liz.Tool/) |
| `Cake.ExtractLicenses` | [link](doc/documenation-cake-addin.md) | [![Latest Release Cake Addin](https://img.shields.io/nuget/v/Cake.ExtractLicenses)](https://www.nuget.org/packages/Cake.ExtractLicenses/) | [![Downloads Cake Addin](https://img.shields.io/nuget/dt/Cake.ExtractLicenses)](https://www.nuget.org/packages/Cake.ExtractLicenses/) |
| `Liz.Nuke` | [link](doc/documentation-nuke-addon.md) | [![Latest Release Nuke Addon](https://img.shields.io/nuget/v/Liz.Nuke)](https://www.nuget.org/packages/Liz.Nuke/) | [![Downloads Nuke Addon](https://img.shields.io/nuget/dt/Liz.Nuke)](https://www.nuget.org/packages/Liz.Nuke/) |

## 🌐 Features

**liz** currently supports the following features:

- Determining all (also with transitive if desired) dependencies for the given solution/project for SDK-style and non-SDK-style projects
- Extract license information (type, URL, text) from all currently known sources for these dependencies
- Print the found dependencies including their license information to the console/log
- Print the problems that occured during the process (missing license-information) to the console/log
- (Try to) determine the license-type from the license-text, if no license-type could be determined
- (Try to) determine the license-type from the license-url, if no license-type could be determined
- Validate the determined package-references and their license-types against a provided whitelist/blacklist
- Export license-information in various forms:
  - license-texts into text-files in a given directory

### Planned features

- [#11](https://github.com/wgnf/liz/issues/11) & [#12](https://github.com/wgnf/liz/issues/12) Mapping from package-reference to license-information
- [#16](https://github.com/wgnf/liz/issues/16) Export license-information in various forms to a given directory/file
- [#5](https://github.com/wgnf/liz/issues/5) & [#7](https://github.com/wgnf/liz/issues/7) Filter for projects and dependencies
- [#6](https://github.com/wgnf/liz/issues/6) Ability to provide manual dependencies
- [#1](https://github.com/wgnf/liz/issues/1) Caching for even faster analyzation times
- [#28](https://github.com/wgnf/liz/issues/28) Sanitize HTML-Tags

## 🛠️ Requirements

### SDK-Style projects

Analyzing SDK-Style projects at least requires:

- .NET Core SDK **2.2** (for the `dotnet` CLI)
  - Get it here:
    - [Official Website](https://dotnet.microsoft.com/en-us/download/dotnet/2.2)
    - [Chocolatey](https://community.chocolatey.org/packages/dotnetcore-2.2-sdk)
- It has to be globally available through the command line (verify with `dotnet --version`)

### Non-SDK-Style projects

Analyzing Non-SDK-Style projects at least requires:

- NuGet CLI (aka `nuget.exe`) **2.7**
  - Get it here:
    - [Official Website](https://www.nuget.org/downloads)
    - [Chocolatey](https://community.chocolatey.org/packages/NuGet.CommandLine)
- It has to be globally available through the command line (verify with `nuget help`)

## ⌨️ Developing

To develop and work with **liz** you just need to clone this Repo somewhere on your PC and then open the Solution or the complete Source-Folder (under `src`) with your favorite IDE. No additional tools required.  
  
Before you can start, you should restore all NuGet-Packages using `dotnet restore` if that's not done for you by your IDE.  
  
As this uses .NET 6.0, you need to install the .NET 6.0.x SDK (as configured by the `global.json`).

## 👋 Want to Contribute?

Cool! We're always welcoming anyone that wants to contribute to this project! Take a look at the [Contributing Guidelines](CONTRIBUTING.md), which helps you get started. You can also look at the [Open Issues](https://github.com/wgnf/liz/issues) for getting more info about current or upcoming tasks.

## 💬 Want to discuss?

If you have any questions, doubts, ideas, problems or you simply want to present your opinions and views, feel free to hop into [Discussions](https://github.com/wgnf/liz/discussions) and write about what you care about. We'd love to hear from you!
