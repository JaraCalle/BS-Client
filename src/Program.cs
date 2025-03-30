using Microsoft.Extensions.DependencyInjection;

class Program
{
    static async Task Main(string[] args)
    {
        // Validar parámetros de log
        if (args.Length == 0)
        {
            Console.WriteLine("Uso: ./client <logfile>");
            Environment.Exit(1);
        }

        string logFilePath = Path.GetFullPath(args[0]);
        
        // Inicializar logger con el archivo especificado
        FileLogger.Initialize(logFilePath);
        AppDomain.CurrentDomain.ProcessExit += (s, e) => FileLogger.Instance?.Dispose();
        
        // Configuración del DI container
        var services = new ServiceCollection();
        
        // Registrar servicios
        services.AddSingleton<INetworkClient>(_ => new NetworkClient("127.0.0.1", 8080));
        services.AddSingleton<Router>();
        
        // Registrar vistas
        services.AddTransient<LoginView>();
        services.AddTransient<LobbyView>();
        services.AddTransient<GameView>();
        
        // Registrar controladores
        services.AddTransient<LoginController>();
        services.AddTransient<LobbyController>();
        services.AddTransient<GameController>();
        
        // Construir el proveedor de servicios
        var serviceProvider = services.BuildServiceProvider();
        
        try
        {
            // Iniciar la aplicación
            Log.Info($"Iniciando aplicación con archivo de log: {logFilePath}");
            var router = serviceProvider.GetRequiredService<Router>();
            await router.NavigateTo<LoginController>();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error fatal: {ex}");
        }
        finally
        {
            // Limpieza
            if (serviceProvider is IDisposable disposable)
            {
                disposable.Dispose();
            }
        }
    }
}