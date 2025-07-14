namespace AzureAppConfigurationDeployment;

public record MatchedKey(
    SettingsKey SourceKey,
    SettingsKey DestinationKey,
    MatchedKeyType MatchType
);

