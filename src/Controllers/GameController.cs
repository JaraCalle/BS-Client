using Spectre.Console;

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
                return;
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
                        await HandleGameOver(parameters[0]);
                        break;

                    case BattleProtocol.ATTACK_RESULT:
                        await HandleAttackResult(parameters);
                        break;
                    
                    case BattleProtocol.TURN:
                        gameSession.IsMyTurn = parameters[0] == player.Name;
                        break;
                }
            }
        }
        catch (Exception e)
        {
            Log.Error("Error Durante el gameLoop", e);
        }
    }

    private async Task HandleGameOver(string winner)
    {
        string gameOverText = winner == player.Name
            ? "[springgreen1 bold]¡Victoria![/] :party_popper:"
            : "[red3_1 bold]¡Derrota![/] :crying_face:";

        AnsiConsole.Clear();
        AnsiConsole.Write(new Panel(gameOverText).Border(BoxBorder.Double).Expand());
        await Task.Delay(TimeSpan.FromSeconds(5));
        await router.NavigateTo<LobbyController>();
    }

    private Task HandleAttackResult(string[] parameters)
    {
        string attackResult = parameters[3];
        string attackerName = parameters[0];
        int x = int.Parse(parameters[1]);
        int y = int.Parse(parameters[2]);

        if (x == -1 || y == -1)
        {
            return Task.CompletedTask;
        }
        
        string subAttackerName = attackerName == player.Name ? $"[bold green]{attackerName}[/]" : $"[bold red]{attackerName}[/]";
        char xCoord = (char)('A' + x); 
        string historyMessage = $"{subAttackerName}: [[{xCoord}{y+1}]] - {attackResult}";
        gameSession.AttackHistory.Add(historyMessage);

        if (attackerName == player.Name)
        {
            gameSession.OpponentBoard[x, y] = attackResult is "HIT" or "SINK" ? "[green]\u233e[/]" : "[red]\u292b[/]";
            gameSession.IsMyTurn = false;
        }
        else
        {
            gameSession.PlayerBoard[x, y] = attackResult is "HIT" or "SINK" ? "[green]\u233e[/]" : "[red]\u292b[/]";
        }

        return Task.CompletedTask;
    }


    public async Task PlaceShips()
    {
        var playerBoard = gameSession.InitEmptyBoard();
        var ships = await networkClient.SendAndWaitForResponseAsync(BattleProtocol.BuildPlaceShipsMessage(), BattleProtocol.OK);
        foreach (var shipData in ships)
        {
            var parts = shipData.Split(',').Select(int.Parse).ToArray();
            int y1 = parts[0], x1 = parts[1], y2 = parts[2], x2 = parts[3];

            for (int y = Math.Min(y1, y2); y <= Math.Max(y1, y2); y++)
            for (int x = Math.Min(x1, x2); x <= Math.Max(x1, x2); x++)
                playerBoard[y, x] = "[bold green]༄[/]";
        }

        gameSession.SetPlayerBoard(playerBoard);
    }

    public async Task ProcessAttack(string coordinates)
    {
        coordinates = coordinates.ToUpper();
        char letter = coordinates[0];
        string numberPart = coordinates[1..];

        int x = letter - 'A';
        if (!int.TryParse(numberPart, out int y))
        {
            Log.Warning("Número inválido en coordenadas");
            return;
        }

        y -= 1;

        if (x < 0 || x >= 10 || y < 0 || y >= 10)
        {
            Log.Warning("Coordenadas fuera de rango");
            return;
        }

        await networkClient.SendAsync(BattleProtocol.BuildAttackMessage(x, y));
    }

    public async Task Surrender()
    {
        var winner = await networkClient.SendAndWaitForResponseAsync(BattleProtocol.BuildSurrenderMessage(), BattleProtocol.GAME_OVER);
        await HandleGameOver(winner[0]);
    }

    public string[,] GetPlayerBoard()
    {
        return gameSession.PlayerBoard;
    }
    
    public string[,] GetOpponentBoard()
    {
        return gameSession.OpponentBoard;
    }
    
    public List<string> GetAttackHistory()
    {
        return gameSession.AttackHistory;
    }
}
