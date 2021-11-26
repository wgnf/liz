using System;
using System.Collections.Generic;
using System.Linq;

namespace DotnetNugetLicenses.Core.PackageReferences.DotnetCli;

/*
 * Example output see:
 * https://docs.microsoft.com/de-de/dotnet/core/tools/dotnet-list-package#description
 */
internal sealed class ParseDotnetListPackageResult : IParseDotnetListPackageResult
{
    public IEnumerable<PackageReference> Parse(string input)
    {
        if (input == null) throw new ArgumentNullException(nameof(input));

        if (string.IsNullOrWhiteSpace(input))
            return Enumerable.Empty<PackageReference>();

        var lines = SplitIntoLines(input);
        var packageReferences = ProcessLines(lines);

        return packageReferences;
    }

    private static IEnumerable<string> SplitIntoLines(string input)
    {
        var inputSplitIntoLines = input
            .Split("\r\n", StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries)
            .Select(line => line.Trim());
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

    private static void ProcessLine(string line, ParsePackageReferenceContext context,
        List<PackageReference> packageReferences)
    {
        // [ indicates line with Target-Framework
        if (line.StartsWith("["))
            context.CurrentTargetFramework = ParseTargetFramework(line);

        if (line.StartsWith("Top-level Package") || line.StartsWith("Transitive Package"))
            context.CurrentIndexOfResolvedVersionString = DetermineIndexOfResolvedVersionString(line);

        // > indicates line with Package-Reference
        if (!line.StartsWith(">")) return;

        var packageReference = ParsePackageReference(context, line);

        packageReferences.Add(packageReference);
    }

    private static string ParseTargetFramework(string line)
    {
        var targetFramework = string.Concat(
            line
                .SkipWhile(character => character == '[')
                .TakeWhile(character => character != ']'));

        return targetFramework;
    }

    private static int DetermineIndexOfResolvedVersionString(string line)
    {
        var index = line.IndexOf("Resolved", StringComparison.InvariantCultureIgnoreCase);
        return index;
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
         * ... as you can see it's a bit tricky.
         *
         * Getting the Name is easy (assuming that spaces are not allowed for package names):
         * starting after "> " (skip 2) until the first space is seen is the package name.
         *
         * For the resolved package version (which is the interesting one for now), we'd need to get the index of the
         * string 'Resolved' in the line 'Top-level Package' or 'Transitive Package' (because the position can vary).
         * This index is then used to gather the resolved version from the actual package reference string (indicated by ">").
         */

        if (string.IsNullOrWhiteSpace(context.CurrentTargetFramework))
            throw new InvalidOperationException(
                $"Couldn't determine PackageReference for line '{line}' because the target framework is unknown. " +
                "This should never have happened...");

        if (context.CurrentIndexOfResolvedVersionString == null)
            throw new InvalidOperationException(
                "Couldn't determine index of 'Resolved' version string. This should never have happened...");

        var packageName = string.Concat(
            line
                .Skip(2)
                .TakeWhile(character => character != ' '));

        var lastElementsThatContainResolvedVersionCount =
            line.Length - context.CurrentIndexOfResolvedVersionString.Value;
        var resolvedVersion = string.Concat(
            line
                .TakeLast(lastElementsThatContainResolvedVersionCount));

        var packageReference = new PackageReference(packageName, context.CurrentTargetFramework, resolvedVersion);
        return packageReference;
    }

    private class ParsePackageReferenceContext
    {
        public string CurrentTargetFramework { get; set; }

        public int? CurrentIndexOfResolvedVersionString { get; set; }
    }
}
