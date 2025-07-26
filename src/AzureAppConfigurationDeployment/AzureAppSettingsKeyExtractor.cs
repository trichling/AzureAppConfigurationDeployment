using System.Collections.Concurrent;
using System.Text.Json;

using Azure.Data.AppConfiguration;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;

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

        await LoadKeyVaultValues(sourceKeys);

        return sourceKeys;
    }

    private async Task<IEnumerable<SettingsKey>> ExtractKeysFrom(AzureAppSettingsKeySource source)
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

    protected virtual SecretClient CreateKeyVaultClient(Uri vaultUri)
    {
        return new SecretClient(vaultUri, new DefaultAzureCredential());
    }

    private async Task LoadKeyVaultValues(IEnumerable<SettingsKey> keys)
    {
        var keyVaultKeys = keys
            .Where(k => k.IsKeyVaultReference && k.KeyVaultValue != null)
            .ToList();

        if (!keyVaultKeys.Any())
            return;

        var clientCache = new ConcurrentDictionary<string, SecretClient>();
        var tasks = keyVaultKeys.Select(async key =>
        {
            try
            {
                var client = clientCache.GetOrAdd(
                    key.KeyVaultValue!.KeyVaultUri.ToString(),
                    uri => CreateKeyVaultClient(key.KeyVaultValue!.KeyVaultUri)
                );

                var secret = await client.GetSecretAsync(
                    key.KeyVaultValue.Key,
                    string.IsNullOrEmpty(key.KeyVaultValue.Version) ? null : key.KeyVaultValue.Version
                );

                key.KeyVaultValue.Value = secret.Value.Value;
            }
            catch (Exception ex)
            {
                throw new KeyVaultAccessException(
                    $"Failed to access Key Vault secret for key '{key.Key}' in vault '{key.KeyVaultValue!.KeyVaultUri}'",
                    ex
                );
            }
        });

        await Task.WhenAll(tasks);
    }
}
