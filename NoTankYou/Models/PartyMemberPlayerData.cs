using System.Runtime.CompilerServices;
using Dalamud.Memory;
using FFXIVClientStructs.FFXIV.Client.Game.Group;
using NoTankYou.Models.Interfaces;

namespace NoTankYou.Models;

public unsafe class PartyMemberPlayerData : IPlayerData
{
    private readonly PartyMember* partyMember;

    public PartyMemberPlayerData(ref PartyMember partyMemberPointer) => partyMember = (PartyMember*) Unsafe.AsPointer(ref partyMemberPointer);
    
    public bool HasStatus(uint statusId) => partyMember->StatusManager.HasStatus(statusId);
    public uint GetObjectId() => partyMember->ObjectID;
    public string GetName() => MemoryHelper.ReadStringNullTerminated((nint) partyMember->Name);
    public float GetStatusTimeRemaining(uint statusId)
    {
        if (HasStatus(statusId))
        {
            var statusIndex = partyMember->StatusManager.GetStatusIndex(statusId);
            return partyMember->StatusManager.GetRemainingTime(statusIndex);
        }

        return 0.0f;
    }
    public byte GetLevel() => partyMember->Level;
    public bool HasClassJob(uint classJobId) => partyMember->ClassJob == classJobId;
    public bool IsDead() => partyMember->CurrentHP is 0;
    public byte GetClassJob() => partyMember->ClassJob;
}