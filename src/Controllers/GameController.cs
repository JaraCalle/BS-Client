using Spectre.Console;
using Spectre.Console.Rendering;

public class GameController(INetworkClient networkClient, Router router, Player player, GameSession gameSession) : IController
{
    public async Task ExecuteAsync()
    {
        await router.NavigateTo<GameController>();
    }

    public async Task GameLoop()
    {
        Log.Warning(gameSession.OpponentName);
        if (gameSession.IsMyTurn)
        {
            var input = AnsiConsole.Ask<string>("Ingresa coordenadas (ej. A5) o [red]ff[/] para salir:");
            if (input.ToLower() == "ff")
            {
                await Surrender();
            }
            await ProcessAttack(input);
        }
        else
        {
            AnsiConsole.MarkupLine($"Esperando que [green underline]{gameSession.OpponentName}[/] ataque...");
        }
        
        try
        {
            var response = await networkClient.ReceiveAsync();
            var commandStack = BattleProtocol.ParseMessage(response);

            foreach (var (command, parameters) in commandStack)
            {
                switch (command)
                {
                    case BattleProtocol.GAME_OVER:
                        string gameOverText = parameters[0] == player.Name ? 
                            "[springgreen1 bold]¡Victoria![/] :party_popper:" : "[red3_1 bold]¡Derrota![/] :crying_face:";
                        AnsiConsole.Clear();
                        var panel = new Panel(gameOverText)
                            .Border(BoxBorder.Double)
                            .Expand();
                        AnsiConsole.Write(panel);
                        await Task.Delay(TimeSpan.FromSeconds(10));
                        
                        await router.NavigateTo<LobbyController>();
                        break;
                }
            }
        }
        catch (Exception e)
        {
            Log.Error("Error Durante el gameLoop", e);
        }
    }
    
    public async Task ProcessAttack(string coordinates)
    {
        Console.WriteLine($"Atacando {coordinates}");
    }

    public char[,] GetBoard()
    {
        // Obtener estado actual del tablero
        return new char[10,10]; // Ejemplo
    }

    public async Task Surrender()
    {
        await networkClient.SendAndWaitForResponseAsync("SURRENDER|", BattleProtocol.OK);
        await router.NavigateTo<LobbyController>();
    }
}