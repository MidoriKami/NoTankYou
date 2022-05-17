using Dalamud.Game.ClientState.Objects.SubKinds;
using Lumina.Excel.GeneratedSheets;

namespace NoTankYou.Utilities
{
    internal static class PlayerLocator
    {
        public static PlayerCharacter? GetPlayer(int objectId)
        {
            var result = Service.ObjectTable.SearchById((uint) objectId);

            if (result?.GetType() == typeof(PlayerCharacter))
                return result as PlayerCharacter;

            return null;
        }

        public static ClassJob? GetClassJob(int objectId)
        {
            var player = GetPlayer(objectId);
            if (player == null) return null;

            return player.ClassJob.GameData;
        }
    }
}
