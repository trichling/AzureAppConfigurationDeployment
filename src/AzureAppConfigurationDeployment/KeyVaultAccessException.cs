namespace AzureAppConfigurationDeployment;

public class KeyVaultAccessException : Exception
{
    public KeyVaultAccessException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}