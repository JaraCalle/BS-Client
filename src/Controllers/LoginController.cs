public class LoginController : IController
{
    private readonly INetworkClient _networkClient;
    private readonly Router _router;
    
    public LoginController(INetworkClient networkClient, Router router)
    {
        _networkClient = networkClient;
        _router = router;
    }

    public async Task ExecuteAsync()
    {
        await _router.NavigateTo<LoginController>();
    }

    public async Task<bool> LoginAsync(string playerName)
    {
        // 1. Intentar establecer la conexión con un bucle
        while (!_networkClient.IsConnected)
        {
            try
            {
                await _networkClient.ConnectAsync();
                break;
            }
            catch (Exception ex)
            {
                await Task.Delay(2000);
            }
        }

        try
        {
            // 2. Enviar el comando LOGIN
            await _networkClient.SendAsync(BattleProtocol.BuildLoginMessage(playerName));

            // 3. Esperar respuesta OK
            string response = await _networkClient.ReceiveWithTimeout(TimeSpan.FromSeconds(10));
            var (command, parameters) = BattleProtocol.ParseMessage(response);

            // 4. Validación estricta
            if (command != BattleProtocol.OK)
            {
                Log.Warning($"Respuesta inesperada del servidor, se esperaba OK, se recibió {command}");
                return false;
            }
            
            Log.Debug($"<{BattleProtocol.BuildLoginMessage(playerName)}> <{response}>");
            await _router.NavigateTo<LobbyController>();
            return true;
        }
        catch (Exception ex)
        {
            Log.Error($"Error durante la ejecución del programa", ex);
            return false;
        }
    }
}