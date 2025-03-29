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
        catch (Exception) { await Task.Delay(1000); }
    }

    try
    {
        // 2. Enviar el comando LOGIN
        await _networkClient.SendAsync(BattleProtocol.BuildLoginMessage(playerName));

        // 3. Esperar respuesta LOGIN_OK
        string response = await ReceiveWithTimeout(TimeSpan.FromSeconds(10));
        var (command, parameters) = BattleProtocol.ParseMessage(response);

        // 4. Validación estricta
        if (command != BattleProtocol.LOGIN_OK)
        {
            Console.WriteLine($"Error: Respuesta inesperada del servidor. Esperaba LOGIN_OK, recibió {command}");
            return false;
        }

        await _router.NavigateTo<GameController>();
        return true;
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error haciendo el login: {ex.Message}");
        return false;
    }
}

private async Task<string> ReceiveWithTimeout(TimeSpan timeout)
{
    var receiveTask = _networkClient.ReceiveAsync();
    var timeoutTask = Task.Delay(timeout);

    var completedTask = await Task.WhenAny(receiveTask, timeoutTask);

    if (completedTask == timeoutTask)
    {
        throw new TimeoutException("El servidor no respondió a tiempo");
    }

    return receiveTask.Result;
}
}