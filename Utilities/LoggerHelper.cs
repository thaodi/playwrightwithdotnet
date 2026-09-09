using Serilog;

namespace PlaywrightTests.Utilities
{
    public static class LoggerHelper
    {
        static LoggerHelper()
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.Console()
                .WriteTo.File("TestResults/Logs/automation_.log", rollingInterval: RollingInterval.Day)
                .CreateLogger();
        }

        public static void Info(string message) => Log.Information(message);
        public static void Warning(string message) => Log.Warning(message);
        public static void Error(string message, Exception? ex = null) => Log.Error(ex, message);
    }
}