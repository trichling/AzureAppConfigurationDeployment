using Xunit;
using NSubstitute;
using Azure.Data.AppConfiguration;
using AzureAppConfigurationDeployment;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace AzureAppConfigurationDeployment.Test;

public class AzureAppSettingsChangeProcessorTests
{
    private readonly ConfigurationClient _mockClient;
    private readonly AzureAppSettingsChangeProcessor _processor;

    public AzureAppSettingsChangeProcessorTests()
    {
        _mockClient = Substitute.For<ConfigurationClient>();
        var source = new AzureAppSettingsKeySource("myservice:api:", string.Empty, new Uri("https://myconfig.azconfig.io")); 
        _processor = new AzureAppSettingsChangeProcessorForTesting(source, _mockClient);
    }

    [Fact]
    public async Task ProcessChanges_CallsUpdateValueInDestination()
    {
        // Arrange
        var matchedKeyActions = new List<MatchedKeyAction>
        {
            new MatchedKeyAction
            (
                Action: MatchedKeyUpdateAction.UpdateValueInDestination,
                MatchedKey: new MatchedKey
                (
                    DestinationKey: new AzureAppSettingsKey
                    (
                        "myservice:api",
                        "TestLabel",
                        "TestKey",
                        "TestValue",
                        string.Empty
                    ),
                    SourceKey: new ConfigurationKey
                    (
                        Key: "TestKey",
                        Label: "TestLabel",
                        Value: "NewTestValue"
                    ),
                    MatchType: MatchedKeyType.ExactMatch
                )
            )
        };

        // Act
        await _processor.ProcessChanges(matchedKeyActions);

        // Assert
        await _mockClient.Received(1).SetConfigurationSettingAsync("myservice:api:TestKey", "NewTestValue", "TestLabel");
    }

    [Fact]
    public async Task ProcessChanges_CallsCreateInDestination()
    {
        // Arrange
        var matchedKeyActions = new List<MatchedKeyAction>
        {
            new MatchedKeyAction
            (
                Action: MatchedKeyUpdateAction.CreateInDestination,
                MatchedKey: new MatchedKey
                (
                    DestinationKey: null,
                    SourceKey: new ConfigurationKey
                    (
                        Key: "SourceTestKey",
                        Label: "SourceTestLabel",
                        Value: "SourceTestValue"
                    ),
                    MatchType: MatchedKeyType.MissingInDestinationMatch
                )
            )
        };
        
        // Act
        await _processor.ProcessChanges(matchedKeyActions);

        // Assert
        await _mockClient.Received(1).AddConfigurationSettingAsync("myservice:api:SourceTestKey", "SourceTestValue", "SourceTestLabel");
    }

    [Fact]
    public async Task ProcessChanges_CallsDeleteInDestination()
    {
        // Arrange
                var matchedKeyActions = new List<MatchedKeyAction>
        {
            new MatchedKeyAction
            (
                Action: MatchedKeyUpdateAction.DeleteInDestination,
                MatchedKey: new MatchedKey
                (
                    DestinationKey: new AzureAppSettingsKey
                    (
                        "myservice:api",
                        "DestinationTestLabel",
                        "DestinationTestKey",
                        "TestValue",
                        string.Empty
                    ),
                    SourceKey: null,
                    MatchType: MatchedKeyType.MissingInSourceMatch
                )
            )
        };
       

        // Act
        await _processor.ProcessChanges(matchedKeyActions);

        // Assert
        await _mockClient.Received(1).DeleteConfigurationSettingAsync("myservice:api:DestinationTestKey", "DestinationTestLabel");
    }
}

public class AzureAppSettingsChangeProcessorForTesting : AzureAppSettingsChangeProcessor
{

    public ConfigurationClient Client { get; set; }

    public AzureAppSettingsChangeProcessorForTesting(AzureAppSettingsKeySource source, ConfigurationClient client) : base(source)
    {
        Client = client;
    }

    protected override ConfigurationClient CreateConfigurationClient(Uri endpoint)
    {
        return Client;
    }
}