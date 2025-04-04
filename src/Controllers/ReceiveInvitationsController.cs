public class ReceiveInvitationsController : IController
{
    private readonly Router _router;
    private readonly INetworkClient _networkClient;
    public event Action<string>? OnInvitationReceived;

    public ReceiveInvitationsController(INetworkClient networkClient, Router router)
    {
        _networkClient = networkClient;
        _router = router;
    }
    
    public async Task ExecuteAsync()
    {
        await _router.NavigateTo<ReceiveInvitationsController>();
    }

    public async Task ListenForInvitationsAsync()
    {
        while (true)
        {
            string response = await _networkClient.ReceiveAsync();
            var (command, parameters) = BattleProtocol.ParseMessage(response);

            switch (command)
            {
                case BattleProtocol.INVITE_RECEIVED:
                    string sender = parameters[0];
                    OnInvitationReceived?.Invoke(sender);
                    break;
                
                case BattleProtocol.GAME_START:
                    Log.Debug("Game started");
                    break;
            }
        }
    }

    public async Task AcceptInvitationAsync(string sender, bool accept)
    {
        await _networkClient.SendAsync(BattleProtocol.BuildInvitationAcceptMessage(sender, accept));
    }
}