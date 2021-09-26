using Ardalis.GuardClauses;
using System.Diagnostics.CodeAnalysis;
using System.IO.Abstractions;

namespace DotnetNugetLicenses.Core.Contracts
{
	[ExcludeFromCodeCoverage] // simple DTO
	public sealed class ExtractSettings
	{
		public ExtractSettings(IFileInfo targetFile)
		{
			TargetFile = Guard.Against.Null(targetFile, nameof(targetFile));
		}

		public IFileInfo TargetFile { get; }
	}
}
