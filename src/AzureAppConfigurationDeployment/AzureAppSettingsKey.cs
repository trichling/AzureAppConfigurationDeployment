namespace AzureAppConfigurationDeployment;

public record AzureAppSettingsKey(string KeyPrefix, string Label, string Key, string Value, string MimeType)
{
    public string KeyPrefixWithoutTrailingColon => KeyPrefix.EndsWith(':') ? KeyPrefix.Substring(0, KeyPrefix.Length - 1) : KeyPrefix;

    public string KeyWithKeyPrefix => $"{KeyPrefixWithoutTrailingColon}:{Key}";
    
}
