namespace AzureAppConfigurationDeployment;

public static class ConfigurationKeyToAzureAppSettingsKeyMatcher
{
    public static List<MatchedKey> MatchKeys(
        IEnumerable<ConfigurationKey> sourceKeys,
        IEnumerable<AzureAppSettingsKey> destinationKeys
    )
    {
        var matchResults = new List<MatchedKey>();

        // ExactMatch oder MissingInDestination
        foreach (var sourceKey in sourceKeys)
        {
            var match = destinationKeys.FirstOrDefault(dk =>
                dk.Key == sourceKey.Key && dk.Label == sourceKey.Label
            );
            if (match != null)
            {
                matchResults.Add(new MatchedKey(sourceKey, match, MatchedKeyType.ExactMatch));
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
            var match = sourceKeys.FirstOrDefault(sk =>
                sk.Key == destinationKey.Key && sk.Label == destinationKey.Label
            );
            if (match == null)
            {
                matchResults.Add(
                    new MatchedKey(null, destinationKey, MatchedKeyType.MissingInSourceMatch)
                );
            }
        }

        return matchResults;
    }

    public static List<MatchedKeyAction> AssignUpdateActions(List<MatchedKey> matchedKeys)
    {
        return matchedKeys
            .Select(mk =>
            {
                return mk.MatchType switch
                {
                    MatchedKeyType.ExactMatch => new MatchedKeyAction(
                        mk,
                        MatchedKeyUpdateAction.UpdateValueInDestination
                    ),
                    MatchedKeyType.MissingInDestinationMatch => new MatchedKeyAction(
                        mk,
                        MatchedKeyUpdateAction.CreateInDestination
                    ),
                    MatchedKeyType.MissingInSourceMatch => new MatchedKeyAction(
                        mk,
                        MatchedKeyUpdateAction.Ignore
                    ),
                };
            })
            .ToList();
    }
}
