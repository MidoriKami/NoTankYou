using System.Collections.Generic;
using System.Linq;

namespace NoTankYou.Data.Components
{
    public class BlacklistSettings
    {
        public bool Enabled = false;

        public List<SimpleTerritory> Territories = new();
    }

    public static class BlacklistSettingsExtensions
    {
        public static bool ContainsCurrentZone(this BlacklistSettings settings)
        {
            var currentTerritory = Service.ClientState.TerritoryType;

            return settings.Territories.Any(t => t.TerritoryID == currentTerritory);
        }
    }
}
