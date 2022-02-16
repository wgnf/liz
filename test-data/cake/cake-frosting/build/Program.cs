using System.Threading.Tasks;
using Cake.Common.IO;
using Cake.Core;
using Cake.Core.IO;
using Cake.Frosting;
using Liz.Cake;

// ReSharper disable once CheckNamespace
public static class Program
{
    public static int Main(string[] args)
    {
        return new CakeHost()
            .UseContext<BuildContext>()
            .Run(args);
    }
}

// ReSharper disable once ClassNeverInstantiated.Global
public class BuildContext : FrostingContext
{
    public FilePath TargetFile { get; }
    
    public BuildContext(ICakeContext context)
        : base(context)
    {
        TargetFile = context.File("../../../sln/TestingGround.sln");
    }
}

[TaskName("ExtractLicensesDefault")]
public sealed class ExtractLicensesDefaultTask : AsyncFrostingTask<BuildContext>
{
    public override async Task RunAsync(BuildContext context)
    {
        await context.ExtractLicensesAsync(context.TargetFile);
    }
}

[TaskName("ExtractLicenses")]
public sealed class ExtractLicensesTask : AsyncFrostingTask<BuildContext>
{
    public override async Task RunAsync(BuildContext context)
    {
        var settings = new ExtractLicensesToolSettings
        {
            IncludeTransitiveDependencies = true
        };
        await context.ExtractLicensesAsync(context.TargetFile, settings);
    }
}
