using System.Text.Json;

using Azure.Data.AppConfiguration;
using Azure.Identity;

namespace AzureAppConfigurationDeployment;

public class SettingsKeySerializer
{
    public static async Task<IEnumerable<SettingsKey>> ImportFrom(string fileName)
    {
        var json = await File.ReadAllTextAsync(fileName);
        var sourceKeys = await DeserializeAsync(json);

        return sourceKeys;
    }

    public static async Task ExportTo(IEnumerable<SettingsKey> keys, string fileName)
    {
        var json = await SerializeAsync(keys);
        await File.WriteAllTextAsync(fileName, json);
    }

    public static IEnumerable<SettingsKey> Deserialize(string json)
    {
        var result = JsonSerializer.Deserialize<IEnumerable<SettingsKey>>(
            json,
            new JsonSerializerOptions
            {
                IgnoreReadOnlyFields = true,
                IgnoreReadOnlyProperties = true,
            }
        );

        return result ?? throw new JsonException("Failed to deserialize Azure App Settings keys.");
    }

    public static async Task<IEnumerable<SettingsKey>> DeserializeAsync(string json)
    {
        return await Task.FromResult(Deserialize(json));
    }

    public static string Serialize(IEnumerable<SettingsKey> keys)
    {
        return JsonSerializer.Serialize(
            keys,
            new JsonSerializerOptions
            {
                WriteIndented = true,
                IgnoreReadOnlyFields = true,
                IgnoreReadOnlyProperties = true,
            }
        );
    }

    public static async Task<string> SerializeAsync(IEnumerable<SettingsKey> keys)
    {
        return await Task.FromResult(Serialize(keys));
    }
}