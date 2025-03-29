using Spectre.Console;

public class LoginView : IView
{
    private LoginController _controller = null!;
    
    public void SetController(IController controller) 
    {
        _controller = (LoginController)controller;
    }

    public async Task RenderAsync()
    {
        AnsiConsole.Clear();
        var rule = new Rule("[bold yellow]BATTLESHIP - LOGIN[/]")
            .Centered();
        AnsiConsole.Write(rule);

        var playerName = AnsiConsole.Ask<string>("Ingresa tu nombre de jugador:");
        
        bool success = await _controller.LoginAsync(playerName);
        
        if (!success)
        {
            AnsiConsole.MarkupLine("[red]Error en el login. Intenta nuevamente.[/]");
            await Task.Delay(10000);
            await RenderAsync(); // Recargar vista
        }
    }
}