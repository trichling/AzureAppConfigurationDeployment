
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;

namespace AzureAppConfigurationDeployment;

public class ConfigurationKeyExtractor
{
    private readonly IEnumerable<ConfigurationKeySource> _sources;

    public ConfigurationKeyExtractor(IEnumerable<ConfigurationKeySource> sources)
    {
        _sources = sources;
    }

    public IEnumerable<SourceKey> ExtractKeys()
    {
        var sourceKeys = new List<SourceKey>();
        foreach (var source in _sources)
            sourceKeys.AddRange(ExtractKeysFrom(source));

        return sourceKeys;
    }

    private IEnumerable<SourceKey> ExtractKeysFrom(ConfigurationKeySource source)
    {
        var configurationKeys = new ConfigurationBuilder()
            .Add(source.Source)
            .Build()
            .AsEnumerable();


        return configurationKeys
            .Where(kvp => kvp.Value != null)
            .Select(kvp => new SourceKey(
                source.Label, 
                kvp.Key, 
                kvp.Value,
                "text/plain"));
    }
}