using System;
using System.Threading.Tasks;

// Interfaz para el cliente de red - Permite mockear para pruebas unitarias
public interface INetworkClient : IDisposable
{
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