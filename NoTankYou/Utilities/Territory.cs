namespace NoTankYou.Utilities
{
    internal static class Territory
    {
        public static bool IsPvP()
        {
            var pvpArea = FFXIVClientStructs.FFXIV.Client.Game.GameMain.IsInPvPArea();
            var pvpInstance = FFXIVClientStructs.FFXIV.Client.Game.GameMain.IsInPvPInstance();

            return pvpArea || pvpInstance;
        }
    }
}
