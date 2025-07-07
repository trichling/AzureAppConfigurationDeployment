using Microsoft.Extensions.Configuration;

namespace AzureAppConfigurationDeployment;

public record ConfigurationKeySource(string Label, IConfigurationSource Source);
