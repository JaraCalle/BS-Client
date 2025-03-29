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

    public async Task ListUsersAsync()
    {
        try
        {
            // 1. Solicitar los usuarios al servidor
            await _networkClient.SendAsync(BattleProtocol.BuildUsersListMessage());

            // 2. Esperar respuesta de USERS
            string response = await _networkClient.ReceiveWithTimeout(TimeSpan.FromSeconds(10));
            var (command, parameters) = BattleProtocol.ParseMessage(response);

            // 3. Validación 
            if (command != BattleProtocol.USERS)
            {
                Console.WriteLine($"Error: respuesta inesperada. Esperaba USERS {command}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
        }
    }
}