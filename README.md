# 📃 liz (ˈlɪz)

[![GitHub license](https://img.shields.io/badge/License-MIT-blue.svg)](LICENSE)
[![Uses SemVer 2.0.0](https://img.shields.io/badge/Uses%20SemVer-2.0.0-green)](https://semver.org/spec/v2.0.0.html)
[![Latest Release](https://img.shields.io/github/v/release/wgnf/liz?label=latest%20release&sort=semver)](https://github.com/wgnf/liz/releases)
[![codecov](https://codecov.io/gh/wgnf/liz/branch/main/graph/badge.svg?token=NMGXDYZMFA)](https://codecov.io/gh/wgnf/liz)  
[![GitHub stars](https://img.shields.io/github/stars/wgnf/liz?style=social)](https://github.com/wgnf/liz/stargazers)
[![GitHub forks](https://img.shields.io/github/forks/wgnf/liz?style=social)](https://github.com/wgnf/liz/network/members)
[![GitHub watchers](https://img.shields.io/github/watchers/wgnf/liz?style=social)](https://github.com/wgnf/liz/watchers)

**liz** is going to be a useful tool to extract licenses from your project, whether it's via a `dotnet` CLI-Tool, Cake or Nuke.

## 🌐 Features

Coming Soon ™️

## 🖥️ Usage

Coming Soon ™️

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
  
As this uses .NET 6.0, you need to install the .NET 6.0.102 SDK (as configured by the `global.json`).

## 👋 Want to Contribute?

**DISCLAIMER**  
As this is in really early development, I'd not recommend you contributing yet, because we have to figure stuff out on our own at the moment. (Code-) Structure and processes might change drastically over that time.  
  
Cool! We're always welcoming anyone that wants to contribute to this project! Take a look at the [Contributing Guidelines](CONTRIBUTING.md), which helps you get started. You can also look at the [Open Issues](https://github.com/wgnf/liz/issues) for getting more info about current or upcoming tasks.

## 💬 Want to discuss?

If you have any questions, doubts, ideas, problems or you simply want to present your opinions and views, feel free to hop into [Discussions](https://github.com/wgnf/liz/discussions) and write about what you care about. We'd love to hear from you!
