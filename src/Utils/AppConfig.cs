public class AppConfig
{
    public string Host { get; set; } = "127.0.0.1";
    public int Port { get; set; } = 12345;

    public static AppConfig LoadFromJson(string filePath)
    {
        if (!File.Exists(filePath))
            Log.Warning($"Archivo de configuración no encontrado, {filePath}");

        string json = File.ReadAllText(filePath);
        return System.Text.Json.JsonSerializer.Deserialize<AppConfig>(json)
               ?? throw new InvalidOperationException("Error al deserializar config.json");
    }
}