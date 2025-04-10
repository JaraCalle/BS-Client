using System.Text.Json;
using System.Text.Json.Serialization.Metadata;

public class AppConfig
{
    public string Host { get; set; } = "127.0.0.1";
    public int Port { get; set; } = 8080;

    public static AppConfig LoadFromJson(string filePath)
    {
        if (!File.Exists(filePath))
            Log.Warning($"Archivo de configuración no encontrado, {filePath}");

        string json = File.ReadAllText(filePath);
        
        var options = new JsonSerializerOptions
        {
            TypeInfoResolver = new DefaultJsonTypeInfoResolver()
        };
        
        return JsonSerializer.Deserialize<AppConfig>(json, options)
               ?? throw new InvalidOperationException("Error al deserializar config.json");
    }
}