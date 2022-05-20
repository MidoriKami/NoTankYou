using Dalamud.Game.ClientState.Objects.SubKinds;

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
    }
}
