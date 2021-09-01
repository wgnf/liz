using Ardalis.GuardClauses;
using DotnetNugetLicenses.Core.Contracts;
using System;

namespace DotnetNugetLicenses.Core
{
	public class ExtractLicenses : IExtractLicenses
	{
		public void Extract(ExtractSettings settings)
		{
			Guard.Against.Null(settings, nameof(settings));

			throw new NotImplementedException();
		}
	}
}
