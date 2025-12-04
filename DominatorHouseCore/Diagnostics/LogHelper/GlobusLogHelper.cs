#region

using DominatorHouseCore.Diagnostics.LogHelper;
using Serilog;
using Serilog.Configuration;
using Serilog.Events;
using Serilog.Sinks.SystemConsole.Themes;
using System;
using System.Reflection;

#endregion

namespace DominatorHouseCore.LogHelper
{
    public static class GlobusLogHelper
    {
        static GlobusLogHelper()
        {
            var config = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .Enrich.FromLogContext()
                // Console with color highlighting
                //.WriteTo.Console(
                //    outputTemplate: "[{Timestamp:yyyy-MM-dd HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}",
                //    theme: AnsiConsoleTheme.Code)

                // File log (Info+)
                //.WriteTo.File(
                //    path: $@"{Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)}\Socinator1.0\logs\Socinator_.log",
                //    rollingInterval: RollingInterval.Day,
                //    retainedFileCountLimit: 30,
                //    restrictedToMinimumLevel: LogEventLevel.Information,
                //    outputTemplate: "[{Timestamp:yyyy-MM-dd HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}"
                //)

                // File log (Debug+)
                .WriteTo.File(
                    path: $@"{Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)}\Socinator1.0\logs\SocinatorDebug_.log",
                    rollingInterval: RollingInterval.Day,
                    retainedFileCountLimit: 30,
                    restrictedToMinimumLevel: LogEventLevel.Debug,
                    outputTemplate: "[{Timestamp:yyyy-MM-dd HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}"
                )

                // UI Sink
                .WriteTo.UiLogSink();

            // Try EventLog sink safely
            try
            {
                config.WriteTo.EventLog(
                    source: "Socinator1.0",
                    manageEventSource: true,
                    restrictedToMinimumLevel: LogEventLevel.Error
                );
            }
            catch
            {
                // Skip EventLog if no permissions
                config.WriteTo.File(
                    path: $@"{Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)}\Socinator1.0\logs\SocinatorErrors_.log",
                    rollingInterval: RollingInterval.Day,
                    retainedFileCountLimit: 30,
                    restrictedToMinimumLevel: LogEventLevel.Error,
                    outputTemplate: "[{Timestamp:yyyy-MM-dd HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}"
                );
            }

            Logger = config.CreateLogger();

            // Expose strongly-typed logger for this class
            Logger = Logger.ForContext(MethodBase.GetCurrentMethod()?.DeclaringType ?? typeof(GlobusLogHelper));
        }

        /// <summary>
        /// Singleton Logger Instance
        /// </summary>
        public static ILogger Logger { get; }
        public static class log
        {
            public static void Trace(string message) => Logger.Verbose(message);
            public static void Trace(string message, params object[] args) => Logger.Verbose(message, args);

            public static void Debug(string message) => Logger.Debug(message);
            public static void Debug(string message, params object[] args) => Logger.Debug(message, args);

            public static void Info(string message) => Logger.Information(message);
            public static void Info(string message, params object[] args) => Logger.Information(message, args);

            public static void Warn(string message) => Logger.Warning(message);
            public static void Warn(string message, params object[] args) => Logger.Warning(message, args);

            // 🔹 Exception overloads
            public static void Error(Exception ex) => Logger.Error(ex, ex.Message);
            public static void Error(Exception ex, string message) => Logger.Error(ex, message);
            public static void Error(string message) => Logger.Error(message);
            public static void Error(string message, params object[] args) => Logger.Error(message, args);

            public static void Fatal(Exception ex) => Logger.Fatal(ex, ex.Message);
            public static void Fatal(Exception ex, string message) => Logger.Fatal(ex, message);
            public static void Fatal(string message) => Logger.Fatal(message);
            public static void Fatal(string message, params object[] args) => Logger.Fatal(message, args);
        }
    }

    public static class UiLogSinkExtensions
    {
        public static LoggerConfiguration UiLogSink(
            this LoggerSinkConfiguration loggerSinkConfiguration,
            IFormatProvider formatProvider = null)
        {
            return loggerSinkConfiguration.Sink(new UiLogSink(formatProvider));
        }
    }
}