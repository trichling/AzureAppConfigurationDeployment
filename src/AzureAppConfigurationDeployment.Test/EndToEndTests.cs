using System.Reflection;

using Azure.Core;
using Azure.Identity;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Memory;
using Microsoft.Extensions.Configuration.UserSecrets; 

namespace AzureAppConfigurationDeployment.Test;

public class EndToEndTests
{

    [Fact]
    public async Task ExtractKeysAndAddToKeyVault()
    {
        var userSecrets = new ConfigurationBuilder().AddUserSecrets(Assembly.GetExecutingAssembly()).Build();
        var keyVaultUri = userSecrets["AZURE_APP_CONFIGURATION_URI"];

        var inMemorySource = new MemoryConfigurationSource
        {
            InitialData = new Dictionary<string, string?>
            {
                {"Logging", null},
                {"Logging:LogLevel", null},
                {"Logging:LogLevel:Microsoft.AspNetCore", "Warning"},
                {"Logging:LogLevel:Default", "Information"},
                {"AllowedHosts", "*"}
            }
        };

        var configurationSource = new ConfigurationKeySource(null, inMemorySource);
        var configurationSourceKeyExtractor = new ConfigurationKeyExtractor(new[] { configurationSource });
        var configurationKeys = configurationSourceKeyExtractor.ExtractKeys();

        var appSettingsSource = new AzureAppSettingsKeySource("apetito:aaa:AzureAppConfigurationDeploymentTests:", "", new Uri(keyVaultUri));
        var appSettingskeyExtractor = new AzureAppSettingsKeyExtractor(appSettingsSource);
        var appSettingsKeys = await appSettingskeyExtractor.ExtractKeys();

        var matchedKeys = ConfigurationKeyToAzureAppSettingsKeyMatcher.MatchKeys(configurationKeys, appSettingsKeys);
        var updateActions = ConfigurationKeyToAzureAppSettingsKeyMatcher.AssignUpdateActions(matchedKeys);

        var changeProcessor = new AzureAppSettingsChangeProcessor(appSettingsSource);
        await changeProcessor.ProcessChanges(updateActions);
    }

}