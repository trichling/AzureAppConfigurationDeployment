
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
  
    public async Task<IEnumerable<DestinationKey>> ExtractKeys()
    {
        var sourceKeys = new List<DestinationKey>();

        foreach (var source in _sources)
            sourceKeys.AddRange(await ExtractKeysFrom(source));

        return sourceKeys;
    }

    private async Task<IEnumerable<DestinationKey>> ExtractKeysFrom(AzureAppSettingsKeySource source)
    {
        var client = CreateConfigurationClient(source.Source);

        var settings = client.GetConfigurationSettingsAsync(new SettingSelector()
        {
            LabelFilter = source.Label,
            KeyFilter = source.KeyPrefix + "*"
        });

        var destinationKeys = new List<DestinationKey>();
        await foreach (var setting in settings)
        {
            destinationKeys.Add(new DestinationKey(
                setting.Label,
                setting.Key,
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
