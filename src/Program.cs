using Microsoft.Extensions.DependencyInjection;

class Program
{
    static async Task Main(string[] args)
    {
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