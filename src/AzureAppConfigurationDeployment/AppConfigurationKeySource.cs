namespace AzureAppConfigurationDeployment;

public record AzureAppSettingsKeySource(string KeyPrefix, string Label, Uri Source);