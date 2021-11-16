using Dalamud.Game.ClientState.Party;
using System.Collections.Generic;
using Dalamud.Game.ClientState.Objects;
using Dalamud.Game.ClientState.Objects.Types;
using Dalamud.Game.ClientState.Objects.SubKinds;
using Dalamud.Game.ClientState.Objects.Enums;
using System.Linq;

namespace NoTankYou
{
    public static class PartyOperations
    {
        public static List<PlayerCharacter> GetAllPlayerCharacters()
        {
            var playerList = new List<PlayerCharacter>();

            foreach(var obj in Service.ObjectTable)
            {
                if(obj.ObjectKind == ObjectKind.Player)
                {
                    playerList.Add((PlayerCharacter)obj);
                }
            }

            return playerList;
        }

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

        // Returns a list of all tanks, list will be empty if no tanks are found
        public static List<PartyMember> GetTanksList(PartyList partyList)
        {
            var tankList = new List<PartyMember>();

            foreach (var player in partyList)
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
