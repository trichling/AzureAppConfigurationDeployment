namespace AzureAppConfigurationDeployment;

public static class UpdateActionsToMatchedKeysAssigner
{
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