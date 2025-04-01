public class LobbyController : IController
{
    private readonly INetworkClient _networkClient;
    private readonly Router _router;

    public LobbyController(INetworkClient networkClient, Router router)
    {
        _networkClient = networkClient;
        _router = router;
    }
    
    public async Task ExecuteAsync()
    {
        await _router.NavigateTo<LobbyController>();
    }

    public async Task<string[]> ListUsersAsync()
    {
        try
        {
            // 1. Solicitar los usuarios al servidor
            await _networkClient.SendAsync(BattleProtocol.BuildUsersListMessage());
            
            // 2. Esperar la respuesta del servidor con los parametros
            var (command, parameters) = await WaitOkResponseAsync();
            
            Log.Debug($"<{BattleProtocol.BuildUsersListMessage()}> <{command} {parameters}>");
            
            return parameters;
        }
        catch (Exception ex)
        {
            Log.Error($"Error solicitando la lista de usuarios", ex);
            return Array.Empty<string>();
        }
    }

    public async Task InviteUserAsync(string playerName)
    {
        try
        {
            // 1. Enviar invitación al usuario destino
            await _networkClient.SendAsync(BattleProtocol.BuildInviteMessage(playerName));
            
            // 2. Esperar respuesta de OK
            await WaitOkResponseAsync();
            
            // 3. Esperar Estado de la invitación
            string response = await _networkClient.ReceiveAsync();
            var (command, parameters) = BattleProtocol.ParseMessage(response);
            
            // 4. Validación
            Console.WriteLine(command);

        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    private async Task<(string, string[])> WaitOkResponseAsync()
    {
        try
        {
            // 1. Esperar respuesta de OK
            string response = await _networkClient.ReceiveWithTimeout(TimeSpan.FromSeconds(10));
            var (command, parameters) = BattleProtocol.ParseMessage(response);

            // 2. Validación 
            if (command != BattleProtocol.OK)
            {
                Log.Warning($"Error: respuesta inesperada. Esperaba OK {command}");
            }
            
            return (command, parameters);  
        }
        catch (Exception e)
        {
            Log.Error($"Error esperando la confirmació OK del server", e);
            return ("", Array.Empty<string>());
        }
        
    }
}