using System;
using System.IO;
using System.Runtime.InteropServices.JavaScript;
using System.Threading;

public enum LogLevel
{
    Debug,
    Info,
    Warning,
    Error
}

public sealed class FileLogger : IDisposable
{
    private static FileLogger _instance = null!;
    private static readonly object _lock = new Object();
    private readonly StreamWriter _writer;
    private readonly string _logFilePath;

    public static FileLogger Instance => _instance;

    public static void Initialize(string logFilePath)
    {
        lock (_lock)
        {
            if (_instance == null)
            {
                _instance = new FileLogger(logFilePath);
            }
        }
    }

    private FileLogger(string logFilePath)
    {
        _logFilePath = logFilePath;
        
        // Crear directorio si no existe
        Directory.CreateDirectory(Path.GetDirectoryName(_logFilePath)!);
        
        // Inicializar archivo de log
        _writer = new StreamWriter(_logFilePath, append: true)
        {
            AutoFlush = true
        };
        
        // Escribir encabezado
        _writer.WriteLine($"=== LOG INICIADO {DateTime.Now:dd-MM-yyyy HH:mm:ss} ===");
    }

    public void Log(LogLevel level, string message, Exception exception = null!)
    {
        lock (_lock)
        {
            string timestamp = DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss");
            string logLevel = level.ToString().ToUpper();
            string logMessage = $"[{timestamp}] [{logLevel}] {message}]";

            if (exception != null)
            {
                logMessage += $"\nException: {exception.Message}\nStack Trace: {exception.StackTrace}";
            }
            
            _writer.WriteLine(logMessage);
        }
    }

    public void Dispose()
    {
        _writer?.Dispose();
    }
}