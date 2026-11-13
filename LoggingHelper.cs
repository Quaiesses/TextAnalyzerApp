using Serilog;

namespace TextAnalyzerApp
{
    public static class LoggingHelper
    {
        public static void Info(string message) => Log.Information(message);
        public static void Error(string message) => Log.Error(message);
    }
}
