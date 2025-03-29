// Implementación del protocolo de comunicación personalizado para Battleship
public static class BattleProtocol
{
    // Constantes de comandos
    public const string LOGIN = "LOGIN";
    public const string LOGIN_OK = "LOGIN_OK";
    public const string USER_LIST = "USER_LIST";
    public const string USERS = "USERS";
    public const string USER_CONNECT = "USER_CONNECT";
    public const string USER_DISCONNECT = "USER_DISCONNECT";
    public const string INVITE = "INVITE";
    public const string INVITATION_RECEIVED = "INVITATION_RECEIVED";
    public const string INVITATION_ACCEPTED = "INVITATION_ACCEPTED";
    public const string INVITATION_DECLINE = "INVITATION_DECLINE";
    public const string GAME_START = "GAME_START";
    public const string ATTACK = "ATTACK";
    public const string ATTACK_RESULT = "ATTACK_RESULT";
    public const string OPPONENT_TURN = "OPPONENT_TURN";
    public const string YOUR_TURN = "YOUR_TURN";
    public const string GAME_OVER = "GAME_OVER";
    public const string LOGOUT = "LOGOUT";
    public const string ERROR = "ERROR";

    // Delimitadores
    private const char COMMAND_DELIMITER = '|';
    private const char PARAMS_DELIMITER = '|';

    //Construye mensaje de conexión
    public static string BuildLoginMessage(string playerName)
    {
        return $"{LOGIN}{COMMAND_DELIMITER}{playerName}";
    }

    //Construye mensaje de lista de usuarios
    public static string BuildUsersListMessage()
    {
        return $"{USER_LIST}";
    }

    //Construye mensaje de invitación
    public static string BuildInviteMessage(string playerName)
    {
        return $"{INVITE}{COMMAND_DELIMITER}{playerName}";
    }

    //Construye mensaje de aceptación de invitación
    public static string BuildInvitationAcceptMessage(string playerName)
    {
        return $"{INVITATION_ACCEPTED}{COMMAND_DELIMITER}{playerName}";
    }

    // Construye mensaje de ataque
    public static string BuildAttackMessage(int x, int y)
    {
        return $"{ATTACK}{COMMAND_DELIMITER}{x}{PARAMS_DELIMITER}{y}";
    }

    //Construye mensaje de desconexión
    public static string BuildLogoutMessage(string playerName)
    {
        return $"{LOGOUT}";
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