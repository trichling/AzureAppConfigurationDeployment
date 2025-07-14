using System.Text.Json;

namespace AzureAppConfigurationDeployment.Test;

public class AzureAppSettingsKeySerializerTests
{
    [Fact]
    public async Task SerializeAndDeserialize_RoundTrip_PreservesData()
    {
        // Arrange
        var sourceKeys = new List<SettingsKey>
        {
            new SettingsKey(
                "myservice:api:",
                "Logging:LogLevel:Default",
                "Information",
                string.Empty,
                string.Empty
            ),
            new SettingsKey(
                "myservice:api:",
                "Logging:LogLevel:Microsoft.AspNetCore",
                "Warning",
                string.Empty,
                string.Empty
            ),
            new SettingsKey(
                "myservice:api:",
                "AllowedHosts",
                "*",
                string.Empty,
                string.Empty
            )
        };

        // Act
        await SettingsKeySerializer.ExportTo(sourceKeys, "serialization_test.json");

        Assert.True(File.Exists("serialization_test.json"));
        var jsonFromFile = await File.ReadAllTextAsync("serialization_test.json");
        Assert.False(string.IsNullOrWhiteSpace(jsonFromFile));

        var keysFromFile = await SettingsKeySerializer.ImportFrom("serialization_test.json");

        // Assert
        Assert.Equal(sourceKeys.Count(), keysFromFile.Count());
        Assert.All(keysFromFile, k => Assert.Equal("myservice:api:", k.KeyPrefix));

        // Clean up
        File.Delete("serialization_test.json");
    }
}
