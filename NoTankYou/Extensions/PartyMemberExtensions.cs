using System.Collections.Generic;
using System.Linq;
using Dalamud.Game.ClientState.Party;

namespace NoTankYou.Extensions
{
    internal static class PartyMemberExtensions
    {
        public static bool HasStatus(this PartyMember character, uint statusId)
        {
            return character.Statuses.Any(status => status.StatusId == statusId);
        }

        public static bool HasStatus(this PartyMember character, List<uint> statusList)
        {
            return character.Statuses.Any(status => statusList.Contains(status.StatusId));
        }
    }
}
