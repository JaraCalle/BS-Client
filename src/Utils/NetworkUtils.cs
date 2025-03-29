using System;
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
}