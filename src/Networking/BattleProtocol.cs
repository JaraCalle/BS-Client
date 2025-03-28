// Implementación del protocolo de comunicación personalizado para Battleship
public static class BattleProtocol
{
    // Constantes de comandos
    public const string CONNECT = "CONNECT";
    public const string START = "START";
    public const string ATTACK = "ATTACK";
    public const string RESULT = "RESULT";
    public const string ERROR = "ERROR";

    // Delimitadores
    private const char COMMAND_DELIMITER = '|';
    private const char PARAMS_DELIMITER = '|';

    ///Construye mensaje de conexión
    public static string BuildConnectMessage(string playerName)
    {
        return $"{CONNECT}{COMMAND_DELIMITER}{playerName}";
    }

    // Construye mensaje de ataque
    public static string BuildAttackMessage(int x, int y)
    {
        return $"{ATTACK}{COMMAND_DELIMITER}{x}{PARAMS_DELIMITER}{y}";
    }

    // Parsea un mensaje recibido
    public static (string Command, string[] Parameters) ParseMessage(string message)
    {
        if (string.IsNullOrEmpty(message))
            throw new ArgumentException("Mensaje no puede estar vacío");

        var parts = message.Split(COMMAND_DELIMITER);
        var command = parts[0];
        var parameters = parts.Length > 2 ? parts.SubArray(1, parts.Length - 1) : Array.Empty<string>();

        return (command, parameters);
    }

    public static T[] SubArray<T>(this T[] data, int index, int length)
    {
        T[] result = new T[length];
        Array.Copy(data, index, result, 0, length);
        return result;
    }
}