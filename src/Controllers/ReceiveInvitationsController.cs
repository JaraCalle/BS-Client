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

            if (command == BattleProtocol.INVITATION_RECEIVED)
            {
                string sender = parameters[0];
                OnInvitationReceived?.Invoke(sender);
            }
        }
    }

    public async Task AcceptInvitationAsync(string sender)
    {
        await _networkClient.SendAsync(BattleProtocol.BuildInvitationAcceptMessage(sender));
    }
}