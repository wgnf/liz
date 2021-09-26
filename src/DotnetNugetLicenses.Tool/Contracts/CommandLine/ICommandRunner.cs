using System.IO;

namespace DotnetNugetLicenses.Tool.Contracts.CommandLine
{
	public interface ICommandRunner
	{
		void Run(FileInfo targetFile);
	}
}