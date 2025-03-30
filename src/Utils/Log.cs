public class Log
{
    public static void Debug(string message) => FileLogger.Instance?.Log(LogLevel.Debug, message);    
    public static void Info(string message) => FileLogger.Instance?.Log(LogLevel.Info, message);    
    public static void Warning(string message) => FileLogger.Instance?.Log(LogLevel.Warning, message);    
    public static void Error(string message, Exception ex = null!) => FileLogger.Instance?.Log(LogLevel.Error, message, ex);    
}