public class LoginController(INetworkClient networkClient, Router router, Player player) : IController
{
    public async Task ExecuteAsync()
    {
        await router.NavigateTo<LoginController>();
    }

    public async Task<bool> LoginAsync(string playerName)
    {
        // 1. Intentar establecer la conexión con un bucle
        while (!networkClient.IsConnected)
        {
            try
            {
                await networkClient.ConnectAsync();
                break;
            }
            catch (Exception ex)
            {
                Log.Warning($"Conexión fallida, reintentandolo! {ex}");
                await Task.Delay(2000);
            }
        }

        try
        {
            // 2. Enviar el comando LOGIN
            await networkClient.SendAsync(BattleProtocol.BuildLoginMessage(playerName));

            // 3. Esperar respuesta OK
            string response = await networkClient.ReceiveWithTimeout(TimeSpan.FromSeconds(10));
            var commandStack = BattleProtocol.ParseMessage(response);

            foreach (var (command, parameters) in commandStack)
            {
                if (command == BattleProtocol.OK)
                {
                    player.Name = playerName;
                    Log.Debug($"<IP> <{BattleProtocol.BuildLoginMessage(playerName)}> <{response}>");
                    await router.NavigateTo<LobbyController>();
                    return true;
                }
            }
            return false;
        }
        catch (Exception ex)
        {
            await networkClient.SendAsync(BattleProtocol.BuildLogoutMessage(playerName));
            player.Name = "";
            networkClient.Disconnect();
            Log.Info($"CONEXIÓN: {networkClient.IsConnected}");
            Log.Error($"Error durante la ejecución del programa", ex);
            return false;
        }
    }
}