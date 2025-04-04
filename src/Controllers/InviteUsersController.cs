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
        return await _networkClient.SendAndWaitForResponseAsync(BattleProtocol.BuildUsersListMessage(), BattleProtocol.OK);
    }

    public async Task InviteUserAsync(string playerName)
    {
        await _networkClient.SendAndWaitForResponseAsync(BattleProtocol.BuildInviteMessage(playerName), BattleProtocol.OK);
        string aceptation = "";
        
        try
        {
            do
            {
                string response = await _networkClient.ReceiveWithTimeout(TimeSpan.FromSeconds(60));
                var (command, parameters) = BattleProtocol.ParseMessage(response);
                
                Log.Warning($"ESTA ES LA RESPONSE DEL SERVER {response}");

                if (command == BattleProtocol.INVITE_RESPONSE)
                {
                    aceptation = parameters[0];
                    break;
                }
                
            } while(true);
        }
        catch (Exception e)
        {
            Log.Error($"Error al esperar la respuesta de la invitación", e);
        }

        if (aceptation != "accept")
        {
            return; 
        }
        
        do
        {
            string response = await _networkClient.ReceiveAsync();
            var (command, parameters) = BattleProtocol.ParseMessage(response);
            
            if (command == BattleProtocol.GAME_START)
            {
                Log.Debug("Game started PARA EL CLIENTE QUE ENVIO LA INVITACIÓN");
                break;
            }
            
        } while(true);
        
    }
}