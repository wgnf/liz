using DotnetNugetLicenses.Tool.CommandLine;
using DotnetNugetLicenses.Tool.Contracts.CommandLine;
using Microsoft.Extensions.Logging;
using Serilog;
using Unity;

namespace DotnetNugetLicenses.Tool
{
    public static class ToolRegistrationExtensions
    {
        public static void RegisterToolServices(this IUnityContainer container)
        {
            container.RegisterType<ICommandProvider, CommandProvider>();
            container.RegisterType<ICommandRunner, CommandRunner>();
        }

        public static void RegisterLoggingServices(this IUnityContainer container)
        {
            ConfigureSerilog();
            var loggerFactory = ConfigureLoggingExtensions();

            container.RegisterInstance(typeof(ILoggerFactory), loggerFactory);
            container.RegisterType(typeof(ILogger<>), typeof(Logger<>));
        }

        private static void ConfigureSerilog()
        {
            Log.Logger = new LoggerConfiguration()
                .Enrich.FromLogContext()
                .WriteTo.Console()
                .CreateLogger();
        }

        private static ILoggerFactory ConfigureLoggingExtensions()
        {
            var loggerFactory = LoggerFactory.Create(builder =>
            {
                builder.AddSerilog();
            });

            return loggerFactory;
        }
    }
}
