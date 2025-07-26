using System.Text.Json;
using System.Text.Json.Serialization;

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
        if (!string.IsNullOrEmpty(keyPrefix))
            Key = Key.Replace(keyPrefix ?? "", string.Empty);

        KeyPrefix = keyPrefix;
        MimeType = mimeType;

        if (IsKeyVaultReference)
            KeyVaultValue = new KeyVaultValue(this);
    }

    public string KeyPrefix { get; private set; }
    public string MimeType { get; private set; }

    public string KeyPrefixWithoutTrailingColon =>
        KeyPrefix.EndsWith(':') ? KeyPrefix.Substring(0, KeyPrefix.Length - 1) : KeyPrefix;

    public string KeyWithKeyPrefix => $"{KeyPrefixWithoutTrailingColon}:{Key}";

    public bool IsKeyVaultReference =>
        MimeType == "application/vnd.microsoft.appconfig.keyvaultref+json;charset=utf-8";

    public KeyVaultValue? KeyVaultValue { get; set; }

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

public record KeyVaultValue
{
    public KeyVaultValue(SettingsKey settingsKey)
    {
        ExtractUri(settingsKey.Value);
        ExtractKeyAndVersion(settingsKey.Value);
    }

    private void ExtractKeyAndVersion(string value)
    {
        var keyVaultReference = JsonSerializer.Deserialize<KeyVaultReference>(value);
        if (string.IsNullOrEmpty(keyVaultReference?.Uri))
        {
            Key = string.Empty;
            Version = string.Empty;
            return;
        }
        var uri = new Uri(keyVaultReference.Uri);
        if (uri.Segments.Length > 0)
        {
            var lastSegment = uri.Segments[^1].TrimEnd('/');
            if (Guid.TryParse(lastSegment, out var guid))
            {
                Version = lastSegment;
                Key = uri.Segments.Length > 1 ? uri.Segments[^2].TrimEnd('/') : string.Empty;
            }
            else
            {
                Key = lastSegment;
                Version = string.Empty;
            }
        }
        else
        {
            Key = string.Empty;
            Version = string.Empty;
        }
    }

    private void ExtractUri(string value)
    {
        var keyVaultReference = JsonSerializer.Deserialize<KeyVaultReference>(value);
        var uri = new Uri(keyVaultReference.Uri);
        KeyVaultUri = new Uri(uri.Scheme + "://" + uri.Host);
    }

    public Uri KeyVaultUri { get; set; }
    public string Key { get; set; }

    public string Value { get; set; }
    public string Version { get; set; } = string.Empty;

    private class KeyVaultReference
    {
        [JsonPropertyName("uri")]
        public string Uri { get; set; } = string.Empty;

    }
}