using Ardalis.GuardClauses;
using DotnetNugetLicenses.Core.Contracts;
using System;
using System.IO;
using System.IO.Abstractions;

namespace DotnetNugetLicenses.Core
{
	public sealed class ExtractLicenses : IExtractLicenses
	{
		public void Extract(ExtractSettings settings)
		{
			Guard.Against.Null(settings, nameof(settings));
            
            CheckProvidedTargetFile(settings.TargetFile);
            
            /*
             * -- TODO --
             * 
             * get projects -> sln: Parse + Select csproj/fsproj | csproj: take csproj
             * filter projects (by name with Regex)
             *
             * get dependencies (including/excluding transitive)
             *      SDK-Style csproj: DependencyGraphSpec (+ Lock-File if transitive)
             *      Old-Style csproj: packages.config (which is already including transitive dependencies - warning when transitive=false!!) without development-dependencies!
             * include manual packages
             * filter dependencies (by name/version with Regex)
             * make dependencies unique (according to settings) 
             *
             * get information for dependencies (however this works...) including raw text if requested
             * include manual License-URL to License Mappings
             *
             * validate dependencies with their licenses (with the provided allowed licenses)
             *
             *
             * in Tool (according to parameters):
             * - print Package-Name + License-Type + License-URL
             * - export License-Texts (package-name as the File-Name) into a specified directory
             * - export Package + License-Details in a JSON-Format (including/excluding Raw Text)
             * 
             */
        }

        private static void CheckProvidedTargetFile(IFileSystemInfo targetFile)
        {
            if (!targetFile.Exists)
                throw new FileNotFoundException($"The provided target-file '{targetFile.FullName}' could not be found");

            if (targetFile.Extension is not ("sln" or "csproj" or "fsproj"))
                throw new ArgumentException(
                    $"The provided target-file '{targetFile.FullName}' is not of the right type " +
                    "(only 'sln', 'csproj' and 'fsproj' is supported");
        }
	}
}
