using System.Text.Json;

using Azure;
using Azure.Core;
using Azure.Data.AppConfiguration;

using NSubstitute; // Add this using directive

namespace AzureAppConfigurationDeployment.Test;

public class AzureAppSettingsKeyExtractorTests
{
    private ConfigurationClient _mockConfigurationClient;

    public AzureAppSettingsKeyExtractorTests()
    {
        _mockConfigurationClient = Substitute.For<ConfigurationClient>();

        var page = AsyncPageable<ConfigurationSetting>.FromPages(
            [
                Page<ConfigurationSetting>.FromValues(
                    [
                        new ConfigurationSetting(
                            "myservice:api:Logging:LogLevel:Default",
                            "Information"
                        ),
                        new ConfigurationSetting(
                            "myservice:api:Logging:LogLevel:Microsoft.AspNetCore",
                            "Warning"
                        ),
                        new ConfigurationSetting("myservice:api:AllowedHosts", "*"),
                    ],
                    null,
                    null
                ),
            ]
        );

        _mockConfigurationClient
            .GetConfigurationSettingsAsync(
                Arg.Is<SettingSelector>(s =>
                    s.KeyFilter == "myservice:api:*" && s.LabelFilter == string.Empty
                )
            )
            .Returns(page);
    }

    [Fact]
    public async Task ExtractKeysFromAppSettings_CallsConfigurationClientWithKeyPrefixAndLabelFilter()
    {
        var source = new AzureAppSettingsKeySource(
            "myservice:api:",
            "Development",
            new Uri("https://myappsettings.azconfig.io")
        );

        var keyExtractor = new AzureAppSettingsKeyExtractorForTesting(source)
        {
            ConfigurationClient = _mockConfigurationClient
        };

        _ = await keyExtractor.ExtractKeys();

        _mockConfigurationClient
            .Received(1)
            .GetConfigurationSettingsAsync(
                Arg.Is<SettingSelector>(s =>
                    s.KeyFilter == "myservice:api:*" && s.LabelFilter == "Development"
                )
            );
    }

    [Fact]
    public async Task ExtractKeysFromAppSettings_StripsKeyPrefixFromKeyAndPutsItIntoKeyPrefixProperty()
    {
        var source = new AzureAppSettingsKeySource(
            "myservice:api:",
            string.Empty,
            new Uri("https://myappsettings.azconfig.io")
        );

        var keyExtractor = new AzureAppSettingsKeyExtractorForTesting(source)
        {
            ConfigurationClient = _mockConfigurationClient
        };

        var keys = await keyExtractor.ExtractKeys();

        // Contains only keys with key prefix 'myservice:api:', but key prefix is not included in the key but in the KeyPrefix property
        Assert.NotEmpty(keys);
        Assert.All(keys, k => Assert.DoesNotContain(source.KeyPrefix, k.Key));
        Assert.All(keys, k => Assert.Equal("myservice:api:", k.KeyPrefix));
    }

}

public class AzureAppSettingsKeyExtractorForTesting : AzureAppSettingsKeyExtractor
{
    public required ConfigurationClient ConfigurationClient { get; set; }

    public AzureAppSettingsKeyExtractorForTesting(AzureAppSettingsKeySource source)
        : base(source) { }

    protected override ConfigurationClient CreateConfigurationClient(Uri endpoint)
    {
        return ConfigurationClient;
    }
}
