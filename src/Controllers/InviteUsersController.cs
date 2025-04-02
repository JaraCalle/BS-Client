public class InviteUsersController : IController
{
    private readonly INetworkClient _networkClient;
    private readonly Router _router;

    public InviteUsersController(INetworkClient networkClient, Router router)
    {
        _networkClient = networkClient;
        _router = router;
    }
    
    public async Task ExecuteAsync()
    {
        await _router.NavigateTo<InviteUsersController>();
    }

    public async Task<string[]> GetUserListAsync()
    {
        await _networkClient.SendAsync(BattleProtocol.BuildUsersListMessage());
        string response = await _networkClient.ReceiveWithTimeout(TimeSpan.FromSeconds(5));
        var (command, users) = BattleProtocol.ParseMessage(response);
        
        if (command == BattleProtocol.OK)
        {
            Log.Debug($"<{BattleProtocol.BuildUsersListMessage()}> <{command}>");
            return users;
        }
        
        Log.Warning($"ERROR: RESPUESTA RECIBIDA: <{BattleProtocol.BuildUsersListMessage()}> {command}");
        return Array.Empty<string>();
    }

    public async Task InviteUserAsync(string playerName)
    {
        await _networkClient.SendAsync(BattleProtocol.BuildInviteMessage(playerName));
    }
}