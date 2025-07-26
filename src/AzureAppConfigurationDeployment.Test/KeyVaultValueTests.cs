using Xunit;
using AzureAppConfigurationDeployment;

namespace AzureAppConfigurationDeployment.Test;

public class KeyVaultValueTests
{
    [Fact]
    public void Extracts_Key_And_Uri_From_KeyVaultReference()
    {
        // Arrange
        var keyVaultReferenceJson = "{\"uri\":\"https://kv-ap-ebs-sart-dev-ha.vault.azure.net/secrets/apetito-articlegateway-api-Authentication-Apetito-ClientSecret/947db105-c500-45b0-ab07-ee04beedac15\"}";
        var settingsKey = new SettingsKey(
            keyPrefix: "",
            label: "",
            key: "",
            value: keyVaultReferenceJson,
            mimeType: "application/vnd.microsoft.appconfig.keyvaultref+json;charset=utf-8"
        );

        // Act
        var keyVaultValue = new KeyVaultValue(settingsKey);

        // Assert
        Assert.Equal(new Uri("https://kv-ap-ebs-sart-dev-ha.vault.azure.net"), keyVaultValue.KeyVaultUri);
        Assert.Equal("apetito-articlegateway-api-Authentication-Apetito-ClientSecret", keyVaultValue.Key);
        Assert.Equal("947db105-c500-45b0-ab07-ee04beedac15", keyVaultValue.Version);
    }

    [Fact]
    public void Extracts_Key_And_Uri_From_KeyVaultReference_WithTrailingSlash()
    {
        // Arrange
        var keyVaultReferenceJson = "{\"uri\":\"https://kv-ap-ebs-sart-dev-ha.vault.azure.net/secrets/apetito-articlegateway-api-Authentication-Apetito-ClientSecret/947db105-c500-45b0-ab07-ee04beedac15/\"}";
        var settingsKey = new SettingsKey(
            keyPrefix: "",
            label: "",
            key: "",
            value: keyVaultReferenceJson,
            mimeType: "application/vnd.microsoft.appconfig.keyvaultref+json;charset=utf-8"
        );

        // Act
        var keyVaultValue = new KeyVaultValue(settingsKey);

        // Assert
        Assert.Equal(new Uri("https://kv-ap-ebs-sart-dev-ha.vault.azure.net"), keyVaultValue.KeyVaultUri);
        Assert.Equal("apetito-articlegateway-api-Authentication-Apetito-ClientSecret", keyVaultValue.Key);
        Assert.Equal("947db105-c500-45b0-ab07-ee04beedac15", keyVaultValue.Version);
    }

    [Fact]
    public void Extracts_Key_And_Uri_From_KeyVaultReference_Without_SecretVersion()
    {
        // Arrange
        var keyVaultReferenceJson = "{\"uri\":\"https://kv-ap-ebs-sart-dev-ha.vault.azure.net/secrets/apetito-articlegateway-api-Authentication-Apetito-ClientSecret\"}";
        var settingsKey = new SettingsKey(
            keyPrefix: "",
            label: "",
            key: "",
            value: keyVaultReferenceJson,
            mimeType: "application/vnd.microsoft.appconfig.keyvaultref+json;charset=utf-8"
        );

        // Act
        var keyVaultValue = new KeyVaultValue(settingsKey);

        // Assert
        Assert.Equal("apetito-articlegateway-api-Authentication-Apetito-ClientSecret", keyVaultValue.Key);
        Assert.Equal(new Uri("https://kv-ap-ebs-sart-dev-ha.vault.azure.net"), keyVaultValue.KeyVaultUri);
        Assert.Equal("", keyVaultValue.Version);
    }

    [Fact]
    public void Extracts_Key_And_Uri_From_KeyVaultReference_Without_SecretVersion_WithTrailingSlash()
    {
        // Arrange
        var keyVaultReferenceJson = "{\"uri\":\"https://kv-ap-ebs-sart-dev-ha.vault.azure.net/secrets/apetito-articlegateway-api-Authentication-Apetito-ClientSecret/\"}";
        var settingsKey = new SettingsKey(
            keyPrefix: "",
            label: "",
            key: "",
            value: keyVaultReferenceJson,
            mimeType: "application/vnd.microsoft.appconfig.keyvaultref+json;charset=utf-8"
        );

        // Act
        var keyVaultValue = new KeyVaultValue(settingsKey);

        // Assert
        Assert.Equal("apetito-articlegateway-api-Authentication-Apetito-ClientSecret", keyVaultValue.Key);
        Assert.Equal(new Uri("https://kv-ap-ebs-sart-dev-ha.vault.azure.net"), keyVaultValue.KeyVaultUri);
        Assert.Equal("", keyVaultValue.Version);

    }
}
