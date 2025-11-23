using System.Text;

namespace Fort.MG;

public enum LogLevel
{
    Debug = 0,
    Info = 1,
    Warn = 2,
    Error = 3,
}

public static class Logger
{
    private static readonly object Lock = new object();
    private static readonly string LogFilePath;

    public static Action<LogLevel, string> LogFn = Log;

#if DEBUG
    public static LogLevel MinimumLevel { get; set; } = LogLevel.Debug;
#else
	public static LogLevel MinimumLevel { get; set; } = LogLevel.Info;
#endif

    static Logger()
    {
        string logDir = Path.Combine(AppContext.BaseDirectory, "logs");
        Directory.CreateDirectory(logDir);

        LogFilePath = Path.Combine(logDir, $"log_{DateTime.Now:yyyy-MM-dd_HH-mm-ss}.txt");
    }

    public static void Log(LogLevel level, string message)
    {
        if (level < MinimumLevel)
            return;

        string prefix = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss.ff}] [{level}]: ";
        string logEntry = $"{prefix}{message}";
        var clr = GetLogColor(level);

        lock (Lock)
        {
            Console.ForegroundColor = clr;
            Console.Write(prefix);
            Console.ResetColor();
            Console.WriteLine(message);
            try
            {
                File.AppendAllText(LogFilePath, logEntry + Environment.NewLine, Encoding.UTF8);
            }
            catch
            {
                // ignore
            }
        }
    }

    private static ConsoleColor GetLogColor(LogLevel lvl)
    {
        switch (lvl)
        {
            case LogLevel.Debug: return ConsoleColor.Cyan;
            case LogLevel.Info: return ConsoleColor.Green;
            case LogLevel.Warn: return ConsoleColor.Yellow;
            case LogLevel.Error: return ConsoleColor.Red;
            default: return ConsoleColor.White;
        }
    }

    public static void Debug(string msg) => Log(LogLevel.Debug, msg);
    public static void Info(string msg) => Log(LogLevel.Info, msg);
    public static void Warn(string msg) => Log(LogLevel.Warn, msg);
    public static void Error(string msg) => Log(LogLevel.Error, msg);
}