namespace AzureAppConfigurationDeployment;

public record SettingsKey(
    string Label,
    string Key,
    string Value
)
{
    public SettingsKey(string keyPrefix, string label, string key, string value, string mimeType)
    : this(label, key, value)
    {
        Key = Key.Replace(KeyPrefix, string.Empty);
        KeyPrefix = keyPrefix;
        MimeType = mimeType;
    }
    public string KeyPrefix { get; private set; }
    public string MimeType { get; private set; }

    public string KeyPrefixWithoutTrailingColon =>
        KeyPrefix.EndsWith(':') ? KeyPrefix.Substring(0, KeyPrefix.Length - 1) : KeyPrefix;

    public string KeyWithKeyPrefix => $"{KeyPrefixWithoutTrailingColon}:{Key}";

    public bool IsKeyVaultReference =>
        MimeType == "application/vnd.microsoft.appconfig.keyvaultref+json;charset=utf-8";

    public string ApplyKeyPrefix(string keyPrefix)
    {
        if (!string.IsNullOrEmpty(KeyPrefix))
            return KeyWithKeyPrefix;

        var keyPrefixWithoutTrailingColon = keyPrefix.EndsWith(':')
            ? keyPrefix.Substring(0, keyPrefix.Length - 1)
            : keyPrefix;
        return $"{keyPrefixWithoutTrailingColon}:{Key}";
    }
}
