﻿using DotnetNugetLicenses.Core.Logging;
using System.IO;
using System.Threading.Tasks;

namespace DotnetNugetLicenses.Tool.Contracts.CommandLine;

public interface ICommandRunner
{
    Task RunAsync(FileInfo targetFile, LogLevel logLevel);
}