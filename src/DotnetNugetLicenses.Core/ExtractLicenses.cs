using Ardalis.GuardClauses;
using DotnetNugetLicenses.Core.Contracts;

namespace DotnetNugetLicenses.Core
{
	public sealed class ExtractLicenses : IExtractLicenses
	{
		public void Extract(ExtractSettings settings)
		{
			Guard.Against.Null(settings, nameof(settings));
            
            /*
             * -- TODO --
             * 
             * get projects -> sln: Parse + Select csproj/fsproj | csproj: take csproj
             * filter projects (by name with Regex)
             *
             * get references (including/excluding transitive)
             * include manual packages
             * filter references (by name/version with Regex)
             *
             * get information for references (however this works...) including raw text if requested
             * include manual License-URL to License Mappings
             *
             * validate references with their licenses (with the provided allowed licenses)
             *
             *
             * in Tool (according to parameters):
             * - print Package-Name + License-Type + License-URL
             * - export License-Texts (package-name as the File-Name) into a specified directory
             * - export Package + License-Details in a JSON-Format (including/excluding Raw Text)
             * 
             */
        }
	}
}
