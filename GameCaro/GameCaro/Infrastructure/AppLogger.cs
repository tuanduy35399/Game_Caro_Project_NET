using System;
using System.IO;

namespace GameCaro.Infrastructure
{
    public static class AppLogger
    {
        private static readonly object SyncRoot = new object();
        private static readonly string LogDirectoryPath =
            Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logs");
        private static readonly string LogFile =
            Path.Combine(LogDirectoryPath, "gamecaro.log");

        public static string LogFilePath
        {
            get { return LogFile; }
        }

        public static void Info(string source, string message)
        {
            Write("INFO", source, message, null);
        }

        public static void Error(string source, string message)
        {
            Write("ERROR", source, message, null);
        }

        public static void Error(string source, string message, Exception exception)
        {
            Write("ERROR", source, message, exception);
        }

        private static void Write(string level, string source, string message, Exception exception)
        {
            try
            {
                lock (SyncRoot)
                {
                    if (!Directory.Exists(LogDirectoryPath))
                    {
                        Directory.CreateDirectory(LogDirectoryPath);
                    }

                    using (var writer = new StreamWriter(LogFile, true))
                    {
                        writer.WriteLine("[" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + "] " + level);
                        writer.WriteLine("Source: " + (source ?? "Unknown"));
                        writer.WriteLine("Message: " + (message ?? string.Empty));
                        if (exception != null)
                        {
                            writer.WriteLine("Exception: " + exception);
                        }
                        writer.WriteLine();
                    }
                }
            }
            catch
            {
                // Logging must never throw to the UI flow.
            }
        }
    }
}
