using System.Collections.Generic;

namespace NoTankYou.Data.Components
{
    public class BlacklistSettings
    {
        public bool Enabled = false;

        public List<uint> Territories = new();
    }
    public static class BlacklistSettingsExtensions
    {
        public static bool ContainsCurrentZone(this BlacklistSettings settings)
        {
            var currentTerritory = Service.ClientState.TerritoryType;

            return settings.Territories.Contains(currentTerritory);
        }
    }
}
