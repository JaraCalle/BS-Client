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
    
        // Generar tableros de 10x10 como placeholders
        var playerBoard = GenerateBoard("Jugador");
        var enemyBoard = GenerateBoard("Enemigo");

        // Crear tabla con tres columnas
        var table = new Table()
            .Border(TableBorder.Rounded)
            .AddColumn("[bold cyan]Tu Tablero[/]")
            .AddColumn("[bold red]Tablero Enemigo[/]")
            .AddColumn("[bold yellow]Historial[/]");
        
        // Agregar las filas con los tableros y el historial
        for (int i = 0; i < 10; i++)
        {
            table.AddRow(
                playerBoard[i], 
                enemyBoard[i], 
                "" // Obtener el historial de jugadas
            );
        }
    
        // Mostrar la tabla en consola
        AnsiConsole.Write(table);
    }

    // Función para generar un tablero de 10x10 con placeholders
    private string[] GenerateBoard(string title)
    {
        var rows = new string[10];

        for (int i = 0; i < 10; i++)
        {
            var row = new List<string>();
            for (int j = 0; j < 10; j++)
            {
                if (i == j)
                {
                    row.Add("\ud83d\udea2");   
                }
                else
                {
                    row.Add("[blue]■[/]"); // Puedes cambiarlo a otra representación
                }
            }
            rows[i] = string.Join(" ", row);
        }

        return rows;
    }

}