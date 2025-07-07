using System.ComponentModel;
using Microsoft.Extensions.Configuration.Json;
using Microsoft.Extensions.Configuration.Memory;

namespace AzureAppConfigurationDeployment.Test;

public class ConfigurationKeyExtractorTests
{
    [Fact]
    public void ExtractKeysFromAppSettings_ExtractsNonEmptyKeysOnly()
    {
        var inMemorySource = new MemoryConfigurationSource
        {
            InitialData = new Dictionary<string, string?>
            {
                { "Logging", null },
                { "Logging:LogLevel", null },
                { "Logging:LogLevel:Microsoft.AspNetCore", "Warning" },
                { "Logging:LogLevel:Default", "Information" },
                { "AllowedHosts", "*" },
            },
        };

        var keySource = new ConfigurationKeySource("", inMemorySource);

        var sourceKeyExtractor = new ConfigurationKeyExtractor(new[] { keySource });

        var sourceKeys = sourceKeyExtractor.ExtractKeys();

        Assert.NotEmpty(sourceKeys);
        Assert.Equal(3, sourceKeys.Count());
    }
}
