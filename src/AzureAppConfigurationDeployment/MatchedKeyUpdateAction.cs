namespace AzureAppConfigurationDeployment;

public enum MatchedKeyUpdateAction
{
    Ignore,
    UpdateValueInDestination,
    CreateInDestination,
    CreateInSource,
    DeleteInDestination,
    DeleteInSource,
}
