namespace AzureAppConfigurationDeployment.Test;

public class ConfigurationKeyToAzureAppSettingsKeyMatcherTests
{
    [Fact]
    public void MatchKeys_ExactMatch_ReturnsExactMatch()
    {
        // Arrange
        var sourceKeys = new List<ConfigurationKey>
        {
            new ConfigurationKey("Label1", "Key1", string.Empty)
        };
        var destinationKeys = new List<AzureAppSettingsKey>
        {
            new AzureAppSettingsKey(string.Empty, "Label1", "Key1", string.Empty, string.Empty)
        };

        // Act
        var result = ConfigurationKeyToAzureAppSettingsKeyMatcher.MatchKeys(sourceKeys, destinationKeys);

        // Assert
        Assert.Single(result);
        Assert.Equal(MatchedKeyType.ExactMatch, result.First().MatchType);
    }

    [Fact]
    public void MatchKeys_MissingInDestination_ReturnsMissingInDestinationMatch()
    {
        // Arrange
        var sourceKeys = new List<ConfigurationKey>
        {
            new ConfigurationKey("Label1", "Key1", string.Empty)
        };
        var destinationKeys = new List<AzureAppSettingsKey>();

        // Act
        var result = ConfigurationKeyToAzureAppSettingsKeyMatcher.MatchKeys(sourceKeys, destinationKeys);

        // Assert
        Assert.Single(result);
        Assert.Equal(MatchedKeyType.MissingInDestinationMatch, result.First().MatchType);
    }

    [Fact]
    public void MatchKeys_MissingInSource_ReturnsMissingInSourceMatch()
    {
        // Arrange
        var sourceKeys = new List<ConfigurationKey>();
        var destinationKeys = new List<AzureAppSettingsKey>
        {
            new AzureAppSettingsKey(string.Empty, "Label1", "Key1", string.Empty, string.Empty)
        };

        // Act
        var result = ConfigurationKeyToAzureAppSettingsKeyMatcher.MatchKeys(sourceKeys, destinationKeys);

        // Assert
        Assert.Single(result);
        Assert.Equal(MatchedKeyType.MissingInSourceMatch, result.First().MatchType);
    }

    [Fact]
    public void MatchKeys_EmptyLists_ReturnsEmptyResult()
    {
        // Arrange
        var sourceKeys = new List<ConfigurationKey>();
        var destinationKeys = new List<AzureAppSettingsKey>();

        // Act
        var result = ConfigurationKeyToAzureAppSettingsKeyMatcher.MatchKeys(sourceKeys, destinationKeys);

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public void MatchKeys_MixedMatches_ReturnsOneEntryForEachKey()
    {
        // Arrange
        var sourceKeys = new List<ConfigurationKey>
        {
            new ConfigurationKey("Label1", "Key1", string.Empty),
            new ConfigurationKey("Label1", "Key2", string.Empty)
        };
        var destinationKeys = new List<AzureAppSettingsKey>
        {
            new AzureAppSettingsKey(string.Empty, "Label1", "Key1", string.Empty, string.Empty),
            new AzureAppSettingsKey(string.Empty, "Label1", "Key3", string.Empty, string.Empty)
        };

        // Act
        var result = ConfigurationKeyToAzureAppSettingsKeyMatcher.MatchKeys(sourceKeys, destinationKeys);

        // Assert
        Assert.Equal(3, result.Count);
        Assert.Equal(MatchedKeyType.ExactMatch, result[0].MatchType);
        Assert.Equal(MatchedKeyType.MissingInDestinationMatch, result[1].MatchType);
        Assert.Equal(MatchedKeyType.MissingInSourceMatch, result[2].MatchType);

    }


    [Fact]
    public void AssignUpdateActions_WithExactMatch_AssignsUpdateValueInDestination()
    {
        // Arrange
        var matchedKeys = new List<MatchedKey>
        {
            new MatchedKey(null, null, MatchedKeyType.ExactMatch)
        };

        // Act
        var result = ConfigurationKeyToAzureAppSettingsKeyMatcher.AssignUpdateActions(matchedKeys);

        // Assert
        Assert.Single(result);
        Assert.Equal(MatchedKeyAction.UpdateValueInDestination, result[0].Action);
    }

    [Fact]
    public void AssignUpdateActions_WithMissingInDestinationMatch_AssignsCreateInDestination()
    {
        // Arrange
        var matchedKeys = new List<MatchedKey>
        {
            new MatchedKey(null, null, MatchedKeyType.MissingInDestinationMatch)
        };

        // Act
        var result = ConfigurationKeyToAzureAppSettingsKeyMatcher.AssignUpdateActions(matchedKeys);

        // Assert
        Assert.Single(result);
        Assert.Equal(MatchedKeyAction.CreateInDestination, result[0].Action);
    }

    [Fact]
    public void AssignUpdateActions_WithMissingInSourceMatch_AssignsIgnore()
    {
        // Arrange
        var matchedKeys = new List<MatchedKey>
        {
            new MatchedKey(null, null, MatchedKeyType.MissingInSourceMatch)
        };

        // Act
        var result = ConfigurationKeyToAzureAppSettingsKeyMatcher.AssignUpdateActions(matchedKeys);

        // Assert
        Assert.Single(result);
        Assert.Equal(MatchedKeyAction.Ignore, result[0].Action);
    }

    [Fact]
    public void AssignUpdateActions_WithEmptyList_ReturnsEmptyList()
    {
        // Arrange
        var matchedKeys = new List<MatchedKey>();

        // Act
        var result = ConfigurationKeyToAzureAppSettingsKeyMatcher.AssignUpdateActions(matchedKeys);

        // Assert
        Assert.Empty(result);
    }
}