public class GameController
{
    private readonly INetworkClient _networkClient;
    private readonly string _playerName;

    public GameController(INetworkClient networkClient, string playerName)
    {
        _networkClient = networkClient;
        _playerName = playerName;
    }

public async Task<bool> StartGameAsync()
{
    try
    {
        // 1. Establecer conexión
        await _networkClient.ConnectAsync();
        
        // 2. Enviar comando CONNECT
        await _networkClient.SendAsync(BattleProtocol.BuildConnectMessage(_playerName));
        
        // 3. Esperar respuesta START con timeout
        string response = await ReceiveWithTimeout(TimeSpan.FromSeconds(10));
        var (command, parameters) = BattleProtocol.ParseMessage(response);
        
        // 4. Validación estricta
        if (command != BattleProtocol.START)
        {
            Console.WriteLine($"Error: Respuesta inesperada del servidor. Esperaba 'START', recibió '{command}'");
            return false;
        }
        
        Console.WriteLine("¡Juego iniciado correctamente!");
        return true;
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error inicializando juego: {ex.Message}");
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

    public async Task<bool> SendAttackAsync(int x, int y)
{
    try
    {
        if (!_networkClient.IsConnected)
        {
            Console.WriteLine("No hay conexión con el servidor");
            return false;
        }
        
        // Enviar ataque
        await _networkClient.SendAsync(BattleProtocol.BuildAttackMessage(x, y));
        
        // Esperar y validar respuesta
        string response = await _networkClient.ReceiveAsync();
        var (command, parameters) = BattleProtocol.ParseMessage(response);
        
        if (command != BattleProtocol.RESULT || parameters.Length < 1)
        {
            Console.WriteLine("Respuesta inesperada del servidor");
            return false;
        }
        
        bool hit = parameters[0] == "1";
        string shipType = parameters.Length > 1 ? parameters[1] : "unknown";
        
        Console.WriteLine(hit 
            ? $"¡Impacto en {x},{y} (Barco: {shipType})!" 
            : $"Agua en {x},{y}");
            
        return true;
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error durante el ataque: {ex.Message}");
        return false;
    }
}
}