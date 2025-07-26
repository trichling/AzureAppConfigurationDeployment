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

    public async Task<IEnumerable<SettingsKey>> ExtractKeys()
    {
        var sourceKeys = new List<SettingsKey>();

        sourceKeys.AddRange(await ExtractKeysFrom(_source));

        return sourceKeys;
    }

    private async Task<IEnumerable<SettingsKey>> ExtractKeysFrom(
        AzureAppSettingsKeySource source
    )
    {
        var client = CreateConfigurationClient(source.Source);

        var settings = client.GetConfigurationSettingsAsync(
            new SettingSelector() { LabelFilter = source.Label, KeyFilter = source.KeyPrefix + "*" }
        );


        var destinationKeys = new List<SettingsKey>();
        await foreach (var setting in settings)
        {
            destinationKeys.Add(
                new SettingsKey(
                    source.KeyPrefix,
                    setting.Label,
                    setting.Key,
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
