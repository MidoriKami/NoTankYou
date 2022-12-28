using System.Collections.Generic;
using KamiLib.Configuration;

namespace NoTankYou.Configuration.Components;

public class BlacklistSettings
{
    public Setting<bool> Enabled = new(false);

    public Setting<List<uint>> BlacklistedZoneSettings = new(new List<uint>());
}

public static class BlacklistSettingsExtensions
{
    public static bool ContainsCurrentZone(this BlacklistSettings settings)
    {
        var currentTerritory = Service.ClientState.TerritoryType;

        return settings.BlacklistedZoneSettings.Value.Contains(currentTerritory);
    }
}