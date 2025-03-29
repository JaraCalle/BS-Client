using System;
using System.Threading.Tasks;

// Interfaz para el cliente de red
public interface INetworkClient : IDisposable
{
    // Variable booleana para saber si ya está conectado el socket
    public bool IsConnected { get; }
    // Conectar al servidor
    Task ConnectAsync();
    // Enviar un mensaje al servidor
    Task SendAsync(string message);
    // Recibir un mensaje del servidor
    Task<string> ReceiveAsync();
    // Cerrar la conexión
    void Disconnect();
}