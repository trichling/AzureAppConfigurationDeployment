
using Azure.Data.AppConfiguration;
using Azure.Identity; 

namespace AzureAppConfigurationDeployment;

public class AzureAppSettingsKeyExtractor
{
    private readonly IEnumerable<AzureAppSettingsKeySource> _sources;

    public AzureAppSettingsKeyExtractor(IEnumerable<AzureAppSettingsKeySource> sources)
    {
        _sources = sources;
    }
  
    public async Task<IEnumerable<AzureAppSettingsKey>> ExtractKeys()
    {
        var sourceKeys = new List<AzureAppSettingsKey>();

        foreach (var source in _sources)
            sourceKeys.AddRange(await ExtractKeysFrom(source));

        return sourceKeys;
    }

    private async Task<IEnumerable<AzureAppSettingsKey>> ExtractKeysFrom(AzureAppSettingsKeySource source)
    {
        var client = CreateConfigurationClient(source.Source);

        var settings = client.GetConfigurationSettingsAsync(new SettingSelector()
        {
            LabelFilter = source.Label,
            KeyFilter = source.KeyPrefix + "*"
        });

        var destinationKeys = new List<AzureAppSettingsKey>();
        await foreach (var setting in settings)
        {
            destinationKeys.Add(new AzureAppSettingsKey(
                source.KeyPrefix,
                setting.Label,
                setting.Key.Replace(source.KeyPrefix, string.Empty),
                setting.Value,
                setting.ContentType));
        }

        return destinationKeys;
    }

    protected virtual ConfigurationClient CreateConfigurationClient(Uri endpoint)
    {
        return new ConfigurationClient(endpoint, new DefaultAzureCredential());
    }
}
