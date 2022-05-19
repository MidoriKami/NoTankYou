using NoTankYou.Localization;

namespace NoTankYou.Utilities
{
    internal static class TerritoryIntendedUseHelper
    {
        public static string GetUseDescription(byte territoryIntendedUse)
        {
            return territoryIntendedUse switch
            {
                0 => Strings.Common.Labels.City,
                1 => Strings.Common.Labels.OpenWorld,
                2 => Strings.Common.Labels.Inn,
                3 => Strings.Common.Labels.Dungeon,
                8 => Strings.Common.Labels.AllianceRaid,
                10 => Strings.Common.Labels.Trial,
                13 or 14 => Strings.Common.Labels.Housing,
                16 or 17 => Strings.Common.Labels.Raid,
                30 => Strings.Common.Labels.GrandCompany,
                31 => Strings.Common.Labels.DeepDiveDungeon,
                41 => Strings.Common.Labels.Eureka,
                48 => Strings.Common.Labels.Bozja,

                _ => Strings.Common.Labels.Unknown
            };
        }
    }
}
