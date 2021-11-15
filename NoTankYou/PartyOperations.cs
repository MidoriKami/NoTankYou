using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Dalamud.Game.ClientState.Party;

namespace NoTankYou
{
    public static class PartyOperations
    {

        public static bool IsInParty(PartyList partyList)
        {
            return partyList.Length > 0;
        }

        public static bool IsTank(string name)
        {
            switch (name)
            {
                case "paladin":
                case "dark knight":
                case "warrior":
                case "gunbreaker":
                    return true;
                default:
                    return false;
            }
        }

        public static (bool, PartyMember?) IsTankPresent(PartyList partyList)
        {
            foreach (var player in partyList)
            {
                if (IsTank(player.ClassJob.GameData.Name))
                {
                    return (true, player);
                }
            }

            return (false, null);
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
            switch (skillName)
            {
                case "Iron Will":
                case "Defiance":
                case "Grit":
                case "Royal Guard":
                    return true;
                default:
                    return false;
            }
        }
    }
}
