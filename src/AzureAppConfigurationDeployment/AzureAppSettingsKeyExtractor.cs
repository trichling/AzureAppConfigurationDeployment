using System.Text.Json;
using Azure.Data.AppConfiguration;
using Azure.Identity;

namespace AzureAppConfigurationDeployment;

public class AzureAppSettingsKeyExtractor
{
    private readonly AzureAppSettingsKeySource _source;

    public AzureAppSettingsKeyExtractor(AzureAppSettingsKeySource source)
    {
        _source = source;
    }

    public async Task<IEnumerable<AzureAppSettingsKey>> ExtractKeys()
    {
        var sourceKeys = new List<AzureAppSettingsKey>();

        sourceKeys.AddRange(await ExtractKeysFrom(_source));

        return sourceKeys;
    }

    public async Task<IEnumerable<AzureAppSettingsKey>> ImportFrom(string fileName)
    {
        var json = File.ReadAllText(fileName);

        var sourceKeys = JsonSerializer.Deserialize<IEnumerable<AzureAppSettingsKey>>(json);

        return sourceKeys;
    }

    public async Task Dump(string fileName)
    {
        var keys = await ExtractKeys();
        var json = JsonSerializer.Serialize(
            keys,
            new JsonSerializerOptions
            {
                WriteIndented = true,
                IgnoreReadOnlyFields = true,
                IgnoreReadOnlyProperties = true,
            }
        );

        await File.WriteAllTextAsync(fileName, json);
    }

    private async Task<IEnumerable<AzureAppSettingsKey>> ExtractKeysFrom(
        AzureAppSettingsKeySource source
    )
    {
        var client = CreateConfigurationClient(source.Source);

        var settings = client.GetConfigurationSettingsAsync(
            new SettingSelector() { LabelFilter = source.Label, KeyFilter = source.KeyPrefix + "*" }
        );

        var destinationKeys = new List<AzureAppSettingsKey>();
        await foreach (var setting in settings)
        {
            destinationKeys.Add(
                new AzureAppSettingsKey(
                    source.KeyPrefix,
                    setting.Label,
                    setting.Key.Replace(source.KeyPrefix, string.Empty),
                    setting.Value,
                    setting.ContentType
                )
            );
        }

        return destinationKeys;
    }

    protected virtual ConfigurationClient CreateConfigurationClient(Uri endpoint)
    {
        return new ConfigurationClient(endpoint, new DefaultAzureCredential());
    }
}
