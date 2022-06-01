using Dalamud.Game.ClientState.Objects.SubKinds;

namespace NoTankYou.Utilities
{
    internal static class PlayerLocator
    {
        public static PlayerCharacter? GetPlayer(uint objectId)
        {
            var result = Service.ObjectTable.SearchById(objectId);

            if (result?.GetType() == typeof(PlayerCharacter))
                return result as PlayerCharacter;

            return null;
        }
    }
}
