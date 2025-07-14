namespace AzureAppConfigurationDeployment;


public static class KeyMatcher
{

    public static List<MatchedKey> MatchKeys(
       IEnumerable<SettingsKey> sourceKeys,
       IEnumerable<SettingsKey> destinationKeys
   )
    {
        var matchResults = new List<MatchedKey>();

        // ExactMatch oder MissingInDestination
        foreach (var sourceKey in sourceKeys)
        {
            var matchedDestinationKey = destinationKeys.FirstOrDefault(destinationKey => sourceKey.Key == destinationKey.Key && sourceKey.Label == destinationKey.Label);
            if (matchedDestinationKey != null)
            {
                matchResults.Add(new MatchedKey(sourceKey, matchedDestinationKey, MatchedKeyType.ExactMatch));
            }
            else
            {
                matchResults.Add(
                    new MatchedKey(sourceKey, null, MatchedKeyType.MissingInDestinationMatch)
                );
            }
        }

        // MissingInSource
        foreach (var destinationKey in destinationKeys)
        {
            var matchedSourceKey = sourceKeys.FirstOrDefault(sourceKey =>
                destinationKey.Key == sourceKey.Key && destinationKey.Label == sourceKey.Label
            );
            if (matchedSourceKey == null)
            {
                matchResults.Add(
                    new MatchedKey(null, destinationKey, MatchedKeyType.MissingInSourceMatch)
                );
            }
        }

        return matchResults;
    }
}

