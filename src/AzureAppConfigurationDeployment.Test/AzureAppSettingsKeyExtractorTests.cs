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
        var source = new AzureAppSettingsKeySource("apetito:accountmanagement:api:", string.Empty, new Uri("https://apetitoappsettings-development.azconfig.io"));

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
                new ConfigurationSetting("apetito:accountmanagement:api:ConnectionString", "Server=tcp:apetito.database.windows.net,1433;Initial Catalog=apetito;Persist Security Info=False;User ID=apetito;Password=apetito;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;"),
                new ConfigurationSetting("apetito:accountmanagement:api:LogLevel", "Information"),
                new ConfigurationSetting("apetito:accountmanagement:api:AllowedHosts", "*")
            ], null, null
        )]);

        mockClient.GetConfigurationSettingsAsync(Arg.Any<SettingSelector>()).Returns(page);

        return mockClient;
    }
}