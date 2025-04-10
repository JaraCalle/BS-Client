public class ReceiveInvitationsController(INetworkClient networkClient, Router router, Player player, GameSession gameSession) : IController
{
    public event Action<string>? OnInvitationReceived;

    public async Task ExecuteAsync()
    {
        await router.NavigateTo<ReceiveInvitationsController>();
    }

    public async Task ListenForInvitationsAsync()
    {
        try
        {
            while (true)
            {
                string response = await networkClient.ReceiveWithTimeout(TimeSpan.FromSeconds(30));
                var commandStack = BattleProtocol.ParseMessage(response);

                foreach (var (command, parameters) in commandStack)
                {
                    switch (command)
                    {
                        case BattleProtocol.INVITE_RECEIVED:
                            string sender = parameters[0];
                            OnInvitationReceived?.Invoke(sender);
                            break;
                    
                        case BattleProtocol.TURN:
                            gameSession.IsMyTurn = player.Name == parameters[0];
                            await router.NavigateTo<GameController>();
                            break;
                    }
                }
            }
        }
        catch (Exception)
        {
            await router.NavigateTo<LobbyController>();
        }
    }

    public async Task AcceptInvitationAsync(string sender, bool accept)
    {
        await networkClient.SendAsync(BattleProtocol.BuildInvitationAcceptMessage(sender, accept));
        if (accept)
        {
            gameSession.OpponentName = sender;
        }
    }
}