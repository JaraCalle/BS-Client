// Implementación del protocolo de comunicación personalizado para Battleship
public static class BattleProtocol
{
    // Inicio de sesión
    public const string LOGIN = "LOGIN";
    public const string LOGOUT = "LOGOUT";
    
    //Listar usuario
    public const string USER_LIST = "USER_LIST";
    public const string USER_CONNECT = "USER_CONNECT";
    public const string USER_DISCONNECT = "USER_DISCONNECT";
    
    // Invitaciones
    public const string INVITE = "INVITE_SEND";
    public const string INVITE_RECEIVED = "INVITE_RECEIVE";
    public const string INVITE_ACK = "INVITE_REPLY";
    public const string INVITE_RESPONSE = "INVITE_RESULT";
    
    // Control de la partida
    public const string GAME_START = "GAME_START";
    public const string ATTACK = "ATTACK";
    public const string ATTACK_RESULT = "ATTACK_RESULT";
    public const string TURN = "TURN";
    public const string GAME_OVER = "GAME_OVER";
    public const string ERROR = "ERROR";
    
    // Confirmaciones
    public const string OK = "OK";
    public const string ACK = "ACK";

    // Delimitadores
    private const char COMMAND_DELIMITER = '|';
    private const char PARAMS_DELIMITER = ' ';
    private const string COMMAND_END = "<EOF>";

    //Construye mensaje de conexión
    public static string BuildLoginMessage(string playerName)
    {
        return $"{LOGIN}{COMMAND_DELIMITER}{playerName}{COMMAND_END}";
    }

    //Construye mensaje de lista de usuarios
    public static string BuildUsersListMessage()
    {
        return $"{USER_LIST}{COMMAND_DELIMITER}{COMMAND_END}";
    }

    //Construye mensaje de invitación
    public static string BuildInviteMessage(string playerName)
    {
        return $"{INVITE}{COMMAND_DELIMITER}{playerName}{COMMAND_END}";
    }
    
    //Construye mensaje de aceptación de invitación
    public static string BuildInvitationAckMessage()
    {
        return $"{INVITE_ACK}{COMMAND_DELIMITER}{COMMAND_END}";
    }

    //Construye mensaje de aceptación de invitación
    public static string BuildInvitationAcceptMessage(string playerName, bool isAccept)
    {
        string response = isAccept ? "ACCEPT" : "REJECT";
        return $"{INVITE_ACK}{COMMAND_DELIMITER}{playerName} {response}{COMMAND_END}";
    }

    // Construye mensaje de ataque
    public static string BuildAttackMessage(int x, int y)
    {
        return $"{ATTACK}{COMMAND_DELIMITER}{x}{PARAMS_DELIMITER}{y}{COMMAND_END}";
    }

    //Construye mensaje de desconexión
    public static string BuildLogoutMessage(string playerName)
    {
        return $"{LOGOUT}{COMMAND_DELIMITER}{COMMAND_END}";
    }

    // Parsea los commandos recibidos y retorna una pila
    public static Stack<(string Command, string[] Parameters)> ParseMessage(string message)
    {
        if (string.IsNullOrEmpty(message))
            throw new ArgumentException("El mensaje no puede estar vacío");

        // Separar los diferentes comandos utilizando COMMAND_END
        string[] rawCommands = message.Split(COMMAND_END, StringSplitOptions.RemoveEmptyEntries);
        Stack<(string Command, string[] Parameters)> commandStack = new();

        foreach (var rawCommand in rawCommands)
        {
            var parts = rawCommand.Split(COMMAND_DELIMITER, 2); // Separa solo en el primer '|'
            string command = parts[0];
            string[] parameters = Array.Empty<string>();
        
            if (parts.Length > 1 && !string.IsNullOrWhiteSpace(parts[1]))
            {
                parameters = parts[1].Split(PARAMS_DELIMITER, StringSplitOptions.RemoveEmptyEntries);
            }
        
            commandStack.Push((command, parameters));
        }
    
        return commandStack;
    }

    public static T[] SubArray<T>(this T[] data, int index, int length)
    {
        T[] result = new T[length];
        Array.Copy(data, index, result, 0, length);
        return result;
    }
}