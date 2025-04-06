using System.Net.Http;
using System.Threading.Tasks;

public static class NetworkUtils
{
    public static async Task<string> ReceiveWithTimeout(
        this INetworkClient networkClient, 
        TimeSpan timeout)
    {
        var receiveTask = networkClient.ReceiveAsync();
        var timeoutTask = Task.Delay(timeout);

        var completedTask = await Task.WhenAny(receiveTask, timeoutTask);

        if (completedTask == timeoutTask)
        {
            throw new TimeoutException("El servidor no respondió a tiempo");
        }

        return await receiveTask; // Mejor usar await que .Result
    }
    
    public static async Task<string> GetPublicIpAddressAsync()
    {
        try
        {
            using HttpClient client = new();
            return await client.GetStringAsync("https://api64.ipify.org");
        }
        catch (Exception ex)
        {
            Log.Error("Error obteniendo la IP pública", ex);
            return "Error";
        }
    }

}