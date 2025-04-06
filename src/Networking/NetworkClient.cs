using System;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

// Cliente de red usando sockets Berkeley para comunicación con servidor Battleship
public class NetworkClient : INetworkClient
{
    private Socket _socket;
    private readonly string _serverIp;
    private readonly int _serverPort;
    private const int BUFFER_SIZE = 1024;
    private const string MESSAGE_TERMINATOR = "\n";
    public bool IsConnected => _socket?.Connected ?? false;

    public NetworkClient(string ip, int port)
    {
        _serverIp = ip;
        _serverPort = port;
        _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
    }

    public async Task ConnectAsync()
    {
        try
        {
            var ipAddress = IPAddress.Parse(_serverIp);
            var remoteEp = new IPEndPoint(ipAddress, _serverPort);
            
            Log.Info($"Cliente intentando conectar al servidor {_serverIp}:{_serverPort}");
            await _socket.ConnectAsync(remoteEp);
        }
        catch (Exception ex)
        {
            Log.Error($"Error al conectar al sevidor", ex);
            throw;
        }
    }

    public async Task SendAsync(string message)
    {
        if (!_socket.Connected)
            throw new InvalidOperationException("Socket no conectado");

        var fullMessage = message + MESSAGE_TERMINATOR;
        byte[] byteData = Encoding.UTF8.GetBytes(fullMessage);
        
        await _socket.SendAsync(byteData, SocketFlags.None);
        Log.Debug($"Enviado: <{message}>");
    }
    
    public async Task<string[]> SendAndWaitForResponseAsync(string message, string expectedResponse)
    {
        await SendAsync(message);
        var result = await ReceiveAsync();
        var commandStack = BattleProtocol.ParseMessage(result);

        foreach (var (command, parameters) in commandStack)
        {
            if (command == expectedResponse)
            {
                string localIp = await NetworkUtils.GetPublicIpAddressAsync();
                Log.Debug($"<{localIp}> <{message}> <{result}>");
                return parameters;
            }
        }
        
        Log.Warning("Respuesta inesperada del servidor");
        return [];
    }

    public async Task<string> ReceiveAsync()
    {
        if (!_socket.Connected)
            throw new InvalidOperationException("Socket no conectado");

        byte[] buffer = new byte[BUFFER_SIZE];
        int bytesRead = await _socket.ReceiveAsync(buffer, SocketFlags.None);
        
        string response = Encoding.UTF8.GetString(buffer, 0, bytesRead);
        response = response.Replace(MESSAGE_TERMINATOR, "");
        
        Log.Debug($"Recibido: <{response}>");
        return response;
    }

    public void Disconnect()
    {
        if (_socket.Connected)
        {
            _socket.Shutdown(SocketShutdown.Both);
            _socket.Close();
        }
        Console.WriteLine("Conexión cerrada");
    }

    public void Dispose()
    {
        Disconnect();
        _socket.Dispose();
    }
}