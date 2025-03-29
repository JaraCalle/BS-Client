public class LoginController 
{
    private readonly INetworkClient _networkClient;
    public LoginController(INetworkClient networkClient)
    {
        _networkClient = networkClient;
    }

public async Task<bool> LoginAsync(string playerName)
{
    // 1. Intentar establecer la conexión con un bucle
    while (true)
    {
        try
        {
            await _networkClient.ConnectAsync();
            break;
        }
        catch (Exception) {}
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

        Console.WriteLine($"Bienvenido {playerName}");
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

    return await receiveTask;
}
}