using System.Collections.Generic;

namespace NoTankYou.Data.Components
{
    public class BlacklistSettings
    {
        public bool Enabled = false;

        public List<uint> BlacklistedZones = new();
    }
    public static class BlacklistSettingsExtensions
    {
        public static bool ContainsCurrentZone(this BlacklistSettings settings)
        {
            var currentTerritory = Service.ClientState.TerritoryType;

            return settings.BlacklistedZones.Contains(currentTerritory);
        }
    }
}
