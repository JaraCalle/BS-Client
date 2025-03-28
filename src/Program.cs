using System;
using System.Threading.Tasks;

class Program
{
    static async Task Main(string[] args)
    {
        Console.WriteLine("Battleship Client - Inicializando...");
        
        try
        {
            // Configuración
            const string serverIp = "127.0.0.1";
            const int serverPort = 8080;
            const string playerName = "Player1";

            // Inicialización
            using INetworkClient networkClient = new NetworkClient(serverIp, serverPort);
            var controller = new GameController(networkClient, playerName);
            
            // Intento de inicio con validación
            bool gameStarted = await controller.StartGameAsync();
            
            if (!gameStarted)
            {
                Console.WriteLine("No se pudo iniciar el juego. Verifica el servidor.");
                return;
            }
            
            // Bucle de juego principal
            await GameLoop(controller);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error fatal: {ex.Message}");
        }
        finally
        {
            Console.WriteLine("Aplicación terminada");
        }
    }

    private static async Task GameLoop(GameController controller)
    {
        while (true)
        {
            try
            {
                Console.Write("Ingresa coordenadas (ej. A5) o 'exit' para salir: ");
                var input = Console.ReadLine()?.Trim().ToUpper();
                
                if (input == "EXIT") break;
                if (string.IsNullOrEmpty(input)) continue;
                
                // Validación de entrada
                if (!IsValidCoordinate(input))
                {
                    Console.WriteLine("Formato inválido. Usa letra (A-J) y número (1-10). Ejemplo: A5");
                    continue;
                }
                
                // Convertir coordenadas
                var (x, y) = ParseCoordinate(input);
                
                // Enviar ataque y procesar respuesta
                bool attackResult = await controller.SendAttackAsync(x, y);
                
                if (!attackResult)
                {
                    Console.WriteLine("Error procesando ataque. ¿Continuar? (S/N)");
                    if (Console.ReadLine()?.Trim().ToUpper() != "S") break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }
    }

    // Métodos auxiliares para validación de coordenadas
    private static bool IsValidCoordinate(string input)
    {
        if (input.Length < 2 || input.Length > 3) return false;
        
        char letter = input[0];
        if (letter < 'A' || letter > 'J') return false;
        
        if (!int.TryParse(input.Substring(1), out int number))
            return false;
        
        return number >= 1 && number <= 10;
    }

    private static (int x, int y) ParseCoordinate(string input)
    {
        char letter = input[0];
        int number = int.Parse(input.Substring(1));
        
        // Convertir A-J -> 0-9 y 1-10 -> 0-9
        return (letter - 'A', number - 1);
    }
}