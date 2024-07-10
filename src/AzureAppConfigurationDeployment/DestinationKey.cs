namespace AzureAppConfigurationDeployment;

public record AzureAppSettingsKey(string KeyPrefix, string Label, string Key, string Value, string MimeType);
