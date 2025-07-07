using Azure;
using Azure.Data.AppConfiguration;
using Azure.Identity;

namespace AzureAppConfigurationDeployment;

public class AzureAppSettingsChangeProcessor
{
    private readonly AzureAppSettingsKeySource _source;
    private ConfigurationClient _configurationClient;

    public AzureAppSettingsChangeProcessor(AzureAppSettingsKeySource source)
    {
        _source = source;
    }

    public async Task ProcessChanges(IEnumerable<MatchedKeyAction> matchedKeyActions)
    {
        _configurationClient = CreateConfigurationClient(_source.Source);

        var relevantActions = matchedKeyActions.Where(a =>
            a.Action == MatchedKeyUpdateAction.UpdateValueInDestination
            || a.Action == MatchedKeyUpdateAction.CreateInDestination
            || a.Action == MatchedKeyUpdateAction.DeleteInDestination
        );

        foreach (var action in relevantActions)
        {
            switch (action.Action)
            {
                case MatchedKeyUpdateAction.UpdateValueInDestination:
                    await UpdateValueInDestination(action.MatchedKey);
                    break;
                case MatchedKeyUpdateAction.CreateInDestination:
                    await CreateInDestination(action.MatchedKey);
                    break;
                case MatchedKeyUpdateAction.DeleteInDestination:
                    await DeleteInDestination(action.MatchedKey);
                    break;
            }
        }
    }

    private async Task DeleteInDestination(MatchedKey matchedKey)
    {
        await _configurationClient.DeleteConfigurationSettingAsync(
            matchedKey.DestinationKey.KeyWithKeyPrefix,
            matchedKey.DestinationKey.Label
        );
    }

    private async Task CreateInDestination(MatchedKey matchedKey)
    {
        await _configurationClient.AddConfigurationSettingAsync(
            matchedKey.SourceKey.ApplyKeyPrefix(_source.KeyPrefix),
            matchedKey.SourceKey.Value,
            matchedKey.SourceKey.Label
        );
    }

    private async Task UpdateValueInDestination(MatchedKey matchedKey)
    {
        await _configurationClient.SetConfigurationSettingAsync(
            matchedKey.DestinationKey.KeyWithKeyPrefix,
            matchedKey.SourceKey.Value,
            matchedKey.DestinationKey.Label
        );
    }

    protected virtual ConfigurationClient CreateConfigurationClient(Uri endpoint)
    {
        return new ConfigurationClient(endpoint, new DefaultAzureCredential());
    }
}
