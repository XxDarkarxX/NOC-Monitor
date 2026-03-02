using System.IO;

namespace NocMonitor.Core;

public static class Logger
{
    private static readonly object _lock = new();
    private static readonly string logFile = "logs.txt";

    public static void Log(string message)
    {
        lock (_lock)
        {
            try
            {
                Directory.CreateDirectory(Path.GetDirectoryName(logFile) ?? ".");
                File.AppendAllText(logFile,
                    $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} | {message}{Environment.NewLine}");
            }
            catch
            {
                // En caso de error de IO no reventamos el monitor
            }
        }
    }
}