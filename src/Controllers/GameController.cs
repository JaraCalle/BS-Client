public class GameController
{
    private readonly INetworkClient _networkClient;

    public GameController(INetworkClient networkClient)
    {
        _networkClient = networkClient;
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
        
        if (command != BattleProtocol.ATTACK_RESULT || parameters.Length < 1)
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