using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lumina.Excel.GeneratedSheets;

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
