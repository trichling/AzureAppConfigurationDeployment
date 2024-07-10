namespace AzureAppConfigurationDeployment;

public record MatchedKey(ConfigurationKey SourceKey, AzureAppSettingsKey DestinationKey, MatchedKeyType MatchType);
