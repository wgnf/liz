using DotnetNugetLicenses.Tool.CommandLine;
using DotnetNugetLicenses.Tool.Contracts.CommandLine;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Events;
using System;
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

        public static void RegisterLoggingServices(this IUnityContainer container, LogLevel logLevel)
        {
            if (logLevel != LogLevel.None)
            {
                var serilogLogLevel = MapToSerilogLogLevel(logLevel);
                ConfigureSerilog(serilogLogLevel);
            }
            
            var loggerFactory = ConfigureLoggingExtensions(logLevel);

            container.RegisterInstance(typeof(ILoggerFactory), loggerFactory);
            container.RegisterType(typeof(ILogger<>), typeof(Logger<>));
        }

        private static LogEventLevel MapToSerilogLogLevel(LogLevel logLevel)
        {
            return logLevel switch
            {
                LogLevel.Trace => LogEventLevel.Verbose,
                LogLevel.Debug => LogEventLevel.Debug,
                LogLevel.Information => LogEventLevel.Information,
                LogLevel.Warning => LogEventLevel.Warning,
                LogLevel.Error => LogEventLevel.Error,
                LogLevel.Critical => LogEventLevel.Fatal,
                // NOTE that this will not be reached, because we're not setting up Serilog, when log-level is set to None
                LogLevel.None => LogEventLevel.Information,
                _ => throw new ArgumentOutOfRangeException(nameof(logLevel), logLevel, null)
            };
        }

        private static void ConfigureSerilog(LogEventLevel logLevel)
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Is(logLevel)
                .Enrich.FromLogContext()
                .WriteTo.Console()
                .CreateLogger();
        }

        private static ILoggerFactory ConfigureLoggingExtensions(LogLevel logLevel)
        {
            var loggerFactory = LoggerFactory.Create(builder =>
            {
                builder.AddSerilog();
                builder.SetMinimumLevel(logLevel);
            });

            return loggerFactory;
        }
    }
}
