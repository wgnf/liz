using Liz.Core.PackageReferences.Contracts;
using Liz.Core.PackageReferences.Contracts.Exceptions;
using Liz.Core.PackageReferences.Contracts.Models;

namespace Liz.Core.PackageReferences.DotnetCli;

/*
 * Example output see:
 * https://docs.microsoft.com/de-de/dotnet/core/tools/dotnet-list-package#description
 */
internal sealed class ParseDotnetListPackageResult : IParseDotnetListPackageResult
{
    public IEnumerable<PackageReference> Parse(string input)
    {
        ArgumentNullException.ThrowIfNull(input);

        if (string.IsNullOrWhiteSpace(input))
            return Enumerable.Empty<PackageReference>();

        var lines = SplitIntoLines(input);
        var packageReferences = ProcessLines(lines);

        return packageReferences;
    }

    private static IEnumerable<string> SplitIntoLines(string input)
    {
        var inputSplitIntoLines = input
            .Split("\r\n", StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
        return inputSplitIntoLines;
    }

    private static IEnumerable<PackageReference> ProcessLines(IEnumerable<string> lines)
    {
        var context = new ParsePackageReferenceContext();
        var packageReferences = new List<PackageReference>();

        foreach (var line in lines)
            ProcessLine(line, context, packageReferences);

        return packageReferences;
    }

    private static void ProcessLine(
        string line,
        ParsePackageReferenceContext context,
        ICollection<PackageReference> packageReferences)
    {
        // [ indicates line with Target-Framework
        if (line.StartsWith("["))
            context.CurrentTargetFramework = ParseTargetFramework(line);

        // > indicates line with Package-Reference
        if (!line.StartsWith(">")) return;

        var packageReference = ParsePackageReference(context, line);
        packageReferences.Add(packageReference);
    }

    private static string ParseTargetFramework(string line)
    {
        /*
         * NOTE:
         * These lines look like this:
         * - [netcoreapp2.1]:
         * - [net5.0]:
         */
        var targetFramework = string.Concat(
            line
                .SkipWhile(character => character == '[')
                .TakeWhile(character => character != ']'));

        return targetFramework;
    }

    private static PackageReference ParsePackageReference(ParsePackageReferenceContext context, string line)
    {
        /*
         * NOTE:
         * Following cases have to be considered:
         *
         * Top-Level-Packages:
         *
         * (1)
         * Top-level Package             Requested   Resolved
         * > JetBrains.Annotations       2021.3.0    2021.3.0
         *
         * (2)
         * Top-level Package               Requested   Resolved
         * > Microsoft.NETCore.App   (A)   [2.1.0, )   2.1.0
         *
         * ---
         * 
         * Transitive Packages:
         *
         * (3)
         * Transitive Package                        Resolved
         * > Microsoft.NETCore.Platforms             5.0.0
         *
         * ---
         *
         * So to parse the line of a package reference we're just splitting the string at every space (' ').
         *
         * Example for case (1):
         * - [0] = "> "                             --> indicator for the package-reference line
         * - [1] = "JetBrains.Annotations"          --> package name
         * - [2] = "2021.3.0"                       --> requested version
         * - [3] = "2021.3.0"                       --> resolved version
         *
         * Example for case (2):
         * - [0] = "> "                             --> indicator for the package-reference line
         * - [1] = "Microsoft.NETCore.App"          --> package name
         * - [2] = "(A)"                            --> indicator for automatic resolved package
         * - [2] = "[2.1.0, )"                      --> requested version
         * - [3] = "2.1.0"                          --> resolved version
         *
         * Example for case (3):
         * - [0] = "> "                             --> indicator for the package-reference line
         * - [1] = "Microsoft.NETCore.Platforms"    --> package name
         * - [2] = "5.0.0"                          --> resolved version
         *
         * So for case (2) we basically have to watch out for the "(A)" on position [2] (but this could also be
         * interesting if someone wants to filter out auto-referenced packages).
         * 
         * But basically we can do the following:
         * - package name = second element
         * - resolved version = last element
         */

        if (string.IsNullOrWhiteSpace(context.CurrentTargetFramework))
            throw new GetPackageReferenceFailedException(line, "the target framework is unknown");
            
        var lineSplit = line
            .Split(' ', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);

        var packageName = lineSplit.ElementAtOrDefault(1);
        var resolvedVersion = lineSplit.LastOrDefault();

        if (packageName == null ||
            resolvedVersion == null)
            throw new GetPackageReferenceFailedException(line, "could not determine package-name or resolved-version");

        var packageReference = new PackageReference(packageName, context.CurrentTargetFramework, resolvedVersion);
        return packageReference;
    }

    private class ParsePackageReferenceContext
    {
        public string CurrentTargetFramework { get; set; } = string.Empty;
    }
}
