using Dalamud.Game.ClientState.Party;
using System.Collections.Generic;

namespace NoTankYou
{
    public static class PartyOperations
    {
        private static readonly List<string> TankClasses = new() { "paladin", "darkknight", "warrior", "gunbreaker" };
        private static readonly List<string> TankStances = new() { "Iron Will", "Defiance", "Grit", "Royal Guard" };

        public static bool IsTank(string name)
        {
            return TankClasses.Contains(name);
        }

        // Returns a list of all tanks, list will be empty if no tanks are found
        public static List<PartyMember> GetTanksList()
        {
            var tankList = new List<PartyMember>();

            foreach (var player in Service.PartyList)
            {
                if (IsTank(player.ClassJob.GameData.Name))
                {
                    tankList.Add(player);
                }
            }

            return tankList;
        }

        public static bool IsTankStanceFound(PartyMember member)
        {
            foreach (var effect in member.Statuses)
            {
                if (IsTankStance(effect.GameData.Name))
                    return true;
            }

            return false;
        }

        public static bool IsTankStance(string skillName)
        {
            return TankStances.Contains(skillName);
        }
    }
}
