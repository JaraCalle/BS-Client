public class ReceiveInvitationsController(INetworkClient networkClient, Router router, Player player, GameSession gameSession) : IController
{
    public event Action<string>? OnInvitationReceived;

    public async Task ExecuteAsync()
    {
        await router.NavigateTo<ReceiveInvitationsController>();
    }

    public async Task ListenForInvitationsAsync()
    {
        while (true)
        {
            string response = await networkClient.ReceiveAsync();
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

    public async Task AcceptInvitationAsync(string sender, bool accept)
    {
        await networkClient.SendAsync(BattleProtocol.BuildInvitationAcceptMessage(sender, accept));
    }
}