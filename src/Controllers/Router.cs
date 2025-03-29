public class Router
{
    private readonly IServiceProvider _services;
    private IView _currentView = null!;

    public Router(IServiceProvider services)
    {
        _services = services;
    }

    public async Task NavigateTo<TController>() where TController : IController
    {
        // Liberar recursos de la vista anterior
        if (_currentView is IDisposable disposable)
        {
            disposable.Dispose();
        }

        // Resolver el controlador
        var controller = (IController)_services.GetService(typeof(TController));
        
        // Resolver la vista asociada
        _currentView = GetViewForController<TController>();
        _currentView.SetController(controller);
        
        // Renderizar vista
        await _currentView.RenderAsync();
    }

    private IView GetViewForController<TController>()
    {
        return typeof(TController).Name switch
        {
            nameof(LoginController) => (IView)_services.GetService(typeof(LoginView)),
            nameof(GameController) => (IView)_services.GetService(typeof(GameView)),
            _ => throw new NotImplementedException($"No hay vista definida para {typeof(TController).Name}")
        };
    }
}
