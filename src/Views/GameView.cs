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
        await _controller.PlaceShips();
        while (true)
        {
            DisplayBoard();
            await _controller.GameLoop();
        }
    }

    private void DisplayBoard()
    {
        AnsiConsole.Clear();

        var panel = new Panel("[bold green]TABLEROS DE JUEGO[/]")
            .Border(BoxBorder.Double)
            .Expand();

        AnsiConsole.Write(panel);

        var table = new Table()
            .Border(TableBorder.Rounded)
            .AddColumn("[bold cyan]Tu Tablero[/]")
            .AddColumn("[bold red]Tablero Enemigo[/]")
            .AddColumn("[bold yellow]Historial[/]");
        
        var history = _controller.GetAttackHistory()
            .TakeLast(10)
            .ToList();

        for (int i = 0; i < 10; i++)
        {
            var playerRow = string.Join(" ", _controller.GetPlayerBoard().GetRow(i));
            var enemyRow = string.Join(" ", _controller.GetOpponentBoard().GetRow(i));
            var historyRow = i < history.Count ? history[i] : "";

            table.AddRow(playerRow, enemyRow, historyRow);
        }

        AnsiConsole.Write(table);
    }
}