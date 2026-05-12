using System.Diagnostics;

namespace ARCbot.Helpers;

public static class Logger
{
    private static readonly string LogDir = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
        "ARCbot", "logs");

    private static readonly object _lock = new();

    static Logger()
    {
        Directory.CreateDirectory(LogDir);
    }

    public static void Info(string message)
    {
        Write("INFO", message);
    }

    public static void Warn(string message)
    {
        Write("WARN", message);
    }

    public static void Error(string message, Exception? ex = null)
    {
        var msg = ex != null ? $"{message} | {ex.GetType().Name}: {ex.Message}" : message;
        Write("ERROR", msg);
        Debug.WriteLine($"[ARCbot ERROR] {msg}");
    }

    public static void Error(Exception ex, string context = "")
    {
        var msg = string.IsNullOrEmpty(context) 
            ? $"{ex.GetType().Name}: {ex.Message}" 
            : $"[{context}] {ex.GetType().Name}: {ex.Message}";
        Write("ERROR", msg);
        Debug.WriteLine($"[ARCbot ERROR] {msg}");
    }

    private static void Write(string level, string message)
    {
        try
        {
            var logFile = Path.Combine(LogDir, $"arcbot_{DateTime.Now:yyyy-MM-dd}.log");
            var line = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] [{level}] {message}{Environment.NewLine}";

            lock (_lock)
            {
                File.AppendAllText(logFile, line);
            }
        }
        catch
        {
            Debug.WriteLine($"[ARCbot] Failed to write log: {message}");
        }
    }
}