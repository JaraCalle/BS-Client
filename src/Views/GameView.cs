using Spectre.Console;

public class GameView : IView
{
    private GameController _controller = null!;
    
    public void SetController(IController controller)
    {
        _controller = (GameController)controller;
    }

    public async Task RenderAsync()
    {
        while (true)
        {
            AnsiConsole.Clear();
            var panel = new Panel("[bold green]TABLERO DE JUEGO[/]")
                .Border(BoxBorder.Double);
                
            AnsiConsole.Write(panel);
            
            // Mostrar tablero
            DisplayBoard();
            
            var input = AnsiConsole.Ask<string>("Ingresa coordenadas (ej. A5) o [red]exit[/] para salir:");
            
            if (input.ToLower() == "exit")
            {
                await _controller.ExitGame();
                break;
            }
            
            await _controller.ProcessAttack(input);
        }
    }

    private void DisplayBoard()
    {
        AnsiConsole.Write("Tablero cool!");
    }
}