using Liz.Core.CliTool.Contracts;
using Liz.Core.CliTool.Contracts.Exceptions;
using Liz.Core.Logging;
using Liz.Core.Logging.Contracts;
using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.IO.Abstractions;
using System.Threading.Tasks;

namespace Liz.Core.CliTool;

[ExcludeFromCodeCoverage] // hard to test because it directly starts a process
internal sealed class DefaultCliToolExecutor : ICliToolExecutor
{
    private readonly ILogger _logger;
    private readonly IFileSystem _fileSystem;

    public DefaultCliToolExecutor(
        [NotNull] ILogger logger,
        [NotNull] IFileSystem fileSystem)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _fileSystem = fileSystem ?? throw new ArgumentNullException(nameof(fileSystem));
    }
    
    public async Task ExecuteAsync(string fileName, string arguments)
    {
        ArgumentNullException.ThrowIfNull(arguments);
        if (string.IsNullOrWhiteSpace(fileName))
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(fileName));

        _ = await ExecuteWithResultAsync(fileName, arguments);
    }
    
    public async Task<string> ExecuteWithResultAsync(string fileName, string arguments)
    {
        ArgumentNullException.ThrowIfNull(arguments);
        if (string.IsNullOrWhiteSpace(fileName))
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(fileName));

        var result = await ExecuteInternalAsync(fileName, arguments);
        return result;
    }

    private async Task<string> ExecuteInternalAsync([NotNull] string fileName, [NotNull] string arguments)
    {
        var localFileName = GetFilenameFromLocalCliBin(fileName);
        var process = StartProcess(localFileName, arguments);
        if (process == null)
            throw new CliToolExecutionFailedException(
                localFileName,
                arguments, 
                "Tool could not be started properly");

        /*
         * NOTE:
         * According to the documentation at
         * https://docs.microsoft.com/en-us/dotnet/api/system.diagnostics.process.standardoutput?redirectedfrom=MSDN&view=net-6.0#System_Diagnostics_Process_StandardOutput
         * you have to read the StandardOutput/StandardError before waiting or else you can get problems
         * (like an endless wait for the process to finish)
         */
        var standardOutput = await GatherStandardOutput(process);
        var errorOutput = await GatherErrorOutput(process);
        
        await process.WaitForExitAsync();

        // NOTE: We can generally assume that an exit code of 0 indicates the success of a process 
        if (process.ExitCode == 0) return standardOutput;
        
        throw new CliToolExecutionFailedException(localFileName, arguments, process.ExitCode, errorOutput);
    }

    private string GetFilenameFromLocalCliBin(string fileName)
    {
        if (!fileName.EndsWith(".exe"))
            fileName = $"{fileName}.exe";
        
        var localCliBinFileName = _fileSystem.Path.Combine("CliTool", "cli-bin", fileName);
        if (!_fileSystem.File.Exists(localCliBinFileName))
            throw new FileNotFoundException("The cli-tool to execute could not be found in the local cli-bin", 
                localCliBinFileName);
        
        return localCliBinFileName;
    }

    private Process StartProcess(string fileName, string arguments)
    {
        try
        {
            var processStartInfo = new ProcessStartInfo(fileName, arguments)
            {
                CreateNoWindow = true,
                ErrorDialog = false,
                RedirectStandardError = true,
                RedirectStandardInput = false,
                RedirectStandardOutput = true,
                UseShellExecute = false
            };
            
            _logger.LogDebug($"CLI: Executing file '{fileName}' with arguments '{arguments}'...");

            var process = Process.Start(processStartInfo);
            return process;
        }
        catch (Exception ex)
        {
            throw new CliToolExecutionFailedException(fileName, arguments, "Tool could not be started properly", ex);
        }
    }

    private async static Task<string> GatherErrorOutput(Process process)
    {
        var standardError = process.StandardError;
        var errorOutput = await standardError.ReadToEndAsync();

        return errorOutput;
    }

    private async static Task<string> GatherStandardOutput(Process process)
    {
        var standardOutput = process.StandardOutput;
        var standardOutputResult = await standardOutput.ReadToEndAsync();

        return standardOutputResult;
    }
}
