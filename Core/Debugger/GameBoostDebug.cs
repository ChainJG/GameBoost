using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;

namespace GameBoost.Core.Debugger
{
    public static class GameBoostDebug
    {
        private static string? _lastCaller;

        [Conditional("DEBUG")]
        public static void Info(
            string message,
            [CallerMemberName] string caller = "",
            [CallerFilePath] string filePath = "")
        {
            Write("INFO", "ℹ️", message, null, caller, filePath);
        }

        [Conditional("DEBUG")]
        public static void Success(
            string message,
            [CallerMemberName] string caller = "",
            [CallerFilePath] string filePath = "")
        {
            Write("SUCCESS", "✅", message, null, caller, filePath);
        }

        [Conditional("DEBUG")]
        public static void Warning(
            string message,
            Exception? exception = null,
            [CallerMemberName] string caller = "",
            [CallerFilePath] string filePath = "")
        {
            Write("WARNING", "⚠️", message, exception, caller, filePath);
        }

        [Conditional("DEBUG")]
        public static void Error(
            string message,
            Exception? exception = null,
            [CallerMemberName] string caller = "",
            [CallerFilePath] string filePath = "")
        {
            Write("ERROR", "❌", message, exception, caller, filePath);
        }

        private static void Write(
            string level,
            string icon,
            string message,
            Exception? exception,
            string caller,
            string filePath)
        {
#if DEBUG
            string fileName = Path.GetFileName(filePath);
            string time = DateTime.Now.ToString("HH:mm:ss.fff");

            if (_lastCaller != caller)
            {
                Debug.WriteLine("");
                Debug.WriteLine("━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━");
            }

            Debug.WriteLine($"{icon} [{level}] [{time}] {fileName}::{caller}");
            Debug.WriteLine($"Message: {message}");

            if (exception is not null)
            {
                Debug.WriteLine($"Exception: {exception.GetType().Name}");
                Debug.WriteLine($"Details:   {exception.Message}");
            }

            Debug.WriteLine("━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━");

            if (_lastCaller == caller) 
                Debug.WriteLine("");

            _lastCaller = caller;
#endif
        }
    }
}