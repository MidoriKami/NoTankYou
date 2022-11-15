using System.Collections.Generic;
using System.Linq;
using Dalamud.Game.ClientState.Party;

namespace NoTankYou.Utilities;

internal static class PartyListExtensions
{
    public static IEnumerable<PartyMember> Alive(this IEnumerable<PartyMember> list)
    {
        return list.Where(member => member.GameObject != null && !member.GameObject.IsDead);
    }

    public static IEnumerable<PartyMember> WithRole(this IEnumerable<PartyMember> list, uint roleID)
    {
        return list.Where(member => member.ClassJob.GameData?.Role == roleID);
    }

    public static IEnumerable<PartyMember> WithJob(this IEnumerable<PartyMember> list, uint jobID)
    {
        return list.Where(member => member.ClassJob.Id == jobID);
    }

    public static IEnumerable<PartyMember> WithJob(this IEnumerable<PartyMember> list, List<uint> jobList)
    {
        return list.Where(member => jobList.Contains(member.ClassJob.Id));
    }

    public static IEnumerable<PartyMember> WithStatus(this IEnumerable<PartyMember> list, uint statusID)
    {
        return list.Where(member => member.HasStatus(statusID));
    }

    public static IEnumerable<PartyMember> WithStatus(this IEnumerable<PartyMember> list, List<uint> statusList)
    {
        return list.Where(member => member.HasStatus(statusList));
    }
}