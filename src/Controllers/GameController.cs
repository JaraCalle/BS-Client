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
            AnsiConsole.MarkupLine($"Esperando que [green1 underline]{player.Name}[/] ataque...");
            var response = await networkClient.ReceiveWithTimeout(TimeSpan.FromSeconds(30));
            Log.Debug($"Response: {response}");
        }
    }
    
    public async Task ProcessAttack(string coordinates)
    {
        Console.WriteLine($"{gameSession.IsMyTurn}");
        Console.WriteLine($"Atacando {coordinates}");
        await Task.Delay(1000);
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