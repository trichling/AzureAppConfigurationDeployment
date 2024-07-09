using NSubstitute; // Add this using directive

using Azure.Data.AppConfiguration;
using Azure.Core;
using Azure;

namespace AzureAppConfigurationDeployment.Test;


public class AzureAppSettingsKeyExtractorTests
{
    [Fact]
    public async Task ExtractKeysFromAppSettings_ExtractsNonEmptyKeysOnly()
    {
        var source = new AzureAppSettingsKeySource("accountmanagement:api:", string.Empty, new Uri("https://myappsettings.azconfig.io"));

        var keyExtractor = new AzureAppSettingsKeyExtractorForTesting([ source ]);

        var keys = await keyExtractor.ExtractKeys();

        Assert.NotEmpty(keys);
    }
}

public class AzureAppSettingsKeyExtractorForTesting : AzureAppSettingsKeyExtractor
{
    public AzureAppSettingsKeyExtractorForTesting(IEnumerable<AzureAppSettingsKeySource> sources) : base(sources)
    {
    }

    protected override ConfigurationClient CreateConfigurationClient(Uri endpoint)
    {
        var mockClient = Substitute.For<ConfigurationClient>();

        var page = AsyncPageable<ConfigurationSetting>.FromPages([Page<ConfigurationSetting>.FromValues(
            [
                new ConfigurationSetting("accountmanagement:api:ConnectionString", "Server=tcp:someserver,1433;Initial Catalog=accountmanagement;Persist Security Info=False;User ID=secret;"),
                new ConfigurationSetting("accountmanagement:api:LogLevel", "Information"),
                new ConfigurationSetting("accountmanagement:api:AllowedHosts", "*")
            ], null, null
        )]);

        mockClient.GetConfigurationSettingsAsync(Arg.Any<SettingSelector>()).Returns(page);

        return mockClient;
    }
}