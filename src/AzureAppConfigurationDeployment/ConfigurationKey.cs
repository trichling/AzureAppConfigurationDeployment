namespace AzureAppConfigurationDeployment;

public record ConfigurationKey(string Label, string Key, string Value)
{
    public string ApplyKeyPrefix(string keyPrefix)
    {
        var keyPrefixWithoutTrailingColon = keyPrefix.EndsWith(':') ? keyPrefix.Substring(0, keyPrefix.Length - 1) : keyPrefix;
        return $"{keyPrefixWithoutTrailingColon}:{Key}";
    }
}