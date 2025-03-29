public class GameController : IController
{
    private readonly Router _router;
    
    public GameController(Router router)
    {
        _router = router;
    }

    public async Task ExecuteAsync()
    {
        await Task.Delay(10);
        await _router.NavigateTo<GameController>();
    }
    public async Task ProcessAttack(string coordinates)
    {
        await Task.Delay(10);
        Console.WriteLine($"Atacando {coordinates}");
    }

    public char[,] GetBoard()
    {
        // Obtener estado actual del tablero
        return new char[10,10]; // Ejemplo
    }

    public async Task ExitGame()
    {
        await _router.NavigateTo<LoginController>();
    }
}