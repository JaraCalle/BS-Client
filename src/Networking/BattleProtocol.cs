// Implementación del protocolo de comunicación personalizado para Battleship
public static class BattleProtocol
{
    // Constantes de comandos
    public const string LOGIN = "LOGIN";
    public const string USER_LIST = "USER_LIST";
    public const string USER_CONNECT = "USER_CONNECT";
    public const string USER_DISCONNECT = "USER_DISCONNECT";
    public const string INVITE = "INVITE";
    public const string INVITE_RECEIVED = "INVITE_FROM";
    public const string INVITE_ACK = "INVITE_ACK";
    public const string INVITE_RESPONSE = "INVITE_RESPONSE";
    public const string GAME_START = "GAME_START";
    public const string ATTACK = "ATTACK";
    public const string ATTACK_RESULT = "ATTACK_RESULT";
    public const string TURN = "TURN";
    public const string GAME_OVER = "GAME_OVER";
    public const string LOGOUT = "LOGOUT";
    public const string ERROR = "ERROR";
    public const string OK = "OK";
    public const string ACK = "ACK";

    // Delimitadores
    private const char COMMAND_DELIMITER = '|';
    private const char PARAMS_DELIMITER = ' ';

    //Construye mensaje de conexión
    public static string BuildLoginMessage(string playerName)
    {
        return $"{LOGIN}{COMMAND_DELIMITER}{playerName}";
    }

    //Construye mensaje de lista de usuarios
    public static string BuildUsersListMessage()
    {
        return $"{USER_LIST}{COMMAND_DELIMITER}";
    }

    //Construye mensaje de invitación
    public static string BuildInviteMessage(string playerName)
    {
        return $"{INVITE}{COMMAND_DELIMITER}{playerName}";
    }
    
    //Construye mensaje de aceptación de invitación
    public static string BuildInvitationAckMessage()
    {
        return $"{INVITE_ACK}{COMMAND_DELIMITER}";
    }

    //Construye mensaje de aceptación de invitación
    public static string BuildInvitationAcceptMessage(string playerName, bool isAccept)
    {
        string response = isAccept ? "accept" : "reject";
        return $"{INVITE_ACK}{COMMAND_DELIMITER}{playerName} {response}";
    }

    // Construye mensaje de ataque
    public static string BuildAttackMessage(int x, int y)
    {
        return $"{ATTACK}{COMMAND_DELIMITER}{x}{PARAMS_DELIMITER}{y}";
    }

    //Construye mensaje de desconexión
    public static string BuildLogoutMessage(string playerName)
    {
        return $"{LOGOUT}{COMMAND_DELIMITER}";
    }

    // Parsea un mensaje recibido
    public static (string Command, string[] Parameters) ParseMessage(string message)
    {
        if (string.IsNullOrEmpty(message))
            throw new ArgumentException("Mensaje no puede estar vacío");
        
        var parts = message.Split(COMMAND_DELIMITER);
        var command = parts[0];
        string[] parameters = [];
        try
        {
            parameters = parts[1].Split(PARAMS_DELIMITER);
            if (parameters.Contains(""))
            {
                parameters = parameters.SubArray(0, parameters.Length - 1);
            }
        }
        catch
        {
            Log.Warning("El comando llego mal formado falta el '|'");
        }
        
        return (command, parameters);
    }

    public static T[] SubArray<T>(this T[] data, int index, int length)
    {
        T[] result = new T[length];
        Array.Copy(data, index, result, 0, length);
        return result;
    }
}