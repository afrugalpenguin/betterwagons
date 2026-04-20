using System;
using System.IO;

namespace BetterWagons.Helpers
{
    /// <summary>
    /// Bypasses MelonLogger buffering. Writes to UserData/BetterWagons_trace.log
    /// with File.AppendAllText (flushes every line).
    /// </summary>
    public static class TraceLog
    {
        private static readonly string LogPath = Path.Combine("UserData", "BetterWagons_trace.log");
        private static bool _initialized;
        private static readonly object _lock = new object();

        public static void Write(string line)
        {
            lock (_lock)
            {
                try
                {
                    if (!_initialized)
                    {
                        Directory.CreateDirectory("UserData");
                        File.WriteAllText(LogPath, $"=== Session start {DateTime.UtcNow:O} ===\n");
                        _initialized = true;
                    }
                    File.AppendAllText(LogPath, $"[{DateTime.UtcNow:HH:mm:ss.fff}] {line}\n");
                }
                catch { }
            }
        }
    }
}
