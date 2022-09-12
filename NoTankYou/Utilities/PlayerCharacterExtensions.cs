using System.Collections.Generic;
using System.Linq;
using Dalamud.Game.ClientState.Objects.Enums;
using Dalamud.Game.ClientState.Objects.SubKinds;
using Dalamud.Game.ClientState.Objects.Types;

namespace NoTankYou.Utilities;

internal static class PlayerCharacterExtensions
{
    public static bool HasStatus(this PlayerCharacter character, uint statusId)
    {
        return character.StatusList.Any(status => status.StatusId == statusId);
    }

    public static bool HasStatus(this PlayerCharacter character, List<uint> statusList)
    {
        return character.StatusList.Any(status => statusList.Contains(status.StatusId));
    }

    public static int StatusCount(this PlayerCharacter character, List<uint> statusList)
    {
        return character.StatusList.Count(status => statusList.Contains(status.StatusId));
    }

    public static bool HasPet(this PlayerCharacter character)
    {
        var ownedObjects = Service.ObjectTable.Where(obj => obj.OwnerId == character.ObjectId);

        return ownedObjects.Any(obj => obj.ObjectKind == ObjectKind.BattleNpc && (obj as BattleNpc)?.SubKind == (byte) BattleNpcSubKind.Pet);
    }

    public static IEnumerable<PlayerCharacter> Alive(this IEnumerable<PlayerCharacter> list)
    {
        return list.Where(member => member.CurrentHp > 0);
    }

    public static IEnumerable<PlayerCharacter> WithJob(this IEnumerable<PlayerCharacter> list, uint jobID)
    {
        return list.Where(member => member.ClassJob.Id == jobID);
    }

    public static IEnumerable<PlayerCharacter> WithJob(this IEnumerable<PlayerCharacter> list, List<uint> jobList)
    {
        return list.Where(member => jobList.Contains(member.ClassJob.Id));
    }

    public static IEnumerable<PlayerCharacter> WithStatus(this IEnumerable<PlayerCharacter> list, uint statusID)
    {
        return list.Where(member => member.HasStatus(statusID));
    }

    public static IEnumerable<PlayerCharacter> WithStatus(this IEnumerable<PlayerCharacter> list, List<uint> statusList)
    {
        return list.Where(member => member.HasStatus(statusList));
    }
}