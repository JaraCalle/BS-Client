using Spectre.Console;

public class InviteUsersController(INetworkClient networkClient, Router router, Player player, GameSession gameSession) : IController
{
    public async Task ExecuteAsync()
    {
        await router.NavigateTo<InviteUsersController>();
    }

    public async Task<string[]> GetUserListAsync()
    {
        var users = await networkClient.SendAndWaitForResponseAsync(
            BattleProtocol.BuildUsersListMessage(), 
            BattleProtocol.OK
            );
        
        return users.Where(u => u != $"{player.Name}:Available").ToArray();
    }

    public async Task InviteUserAsync(string playerName)
    {
        AnsiConsole.Clear();
        playerName = playerName.Replace(":Available", "");
        AnsiConsole.MarkupLine($"Esperando que [green]{playerName}[/] responda tu invitación...");
        await networkClient.SendAndWaitForResponseAsync(BattleProtocol.BuildInviteMessage(playerName), BattleProtocol.OK);
        string aceptation;
        bool flag = false; 
        try
        {
            do
            {
                string response = await networkClient.ReceiveWithTimeout(TimeSpan.FromSeconds(15));
                var commandStack = BattleProtocol.ParseMessage(response);
                
                foreach (var (command, parameters) in commandStack)
                {
                    switch (command)
                    {
                        case BattleProtocol.INVITE_RESPONSE:
                            aceptation = parameters[1];
                            if (aceptation == "REJECT")
                            {
                                var panel = new Panel($":cross_mark: [bold red]Tristemente {playerName} rechazó tu invitación[/] :cross_mark:")
                                    .Border(BoxBorder.Double)
                                    .Expand();
                                AnsiConsole.Write(panel);
                                
                                flag = true;
                            }
                            break;
                        
                        case BattleProtocol.TURN:
                            flag = true;
                            gameSession.OpponentName = playerName;
                            gameSession.IsMyTurn = player.Name == parameters[0];
                            await router.NavigateTo<GameController>();
                            break;
                    }
                }
                if (flag) break;
                else
                {
                    AnsiConsole.MarkupLine("Esperando que el servidor te responda...");
                }
            } while(true);
        }
        catch (Exception e)
        {
            Log.Error($"Error al esperar la respuesta de la invitación", e);
        }
    }

    public async Task NavigateToLobby()
    {
        await router.NavigateTo<LobbyController>();
    }
}
