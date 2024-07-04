using FFXIVClientStructs.FFXIV.Client.Game.Group;

namespace NoTankYou.PlayerDataInterface;

public unsafe class PartyMemberPlayerData(PartyMember* partyMemberPointer) : IPlayerData {
    public bool HasStatus(uint statusId)
        => partyMemberPointer->StatusManager.HasStatus(statusId);
    
    public uint GetEntityId() 
        => partyMemberPointer->EntityId;
    
    public string GetName() 
        => partyMemberPointer->NameString;
    
    public float GetStatusTimeRemaining(uint statusId) {
        if (HasStatus(statusId)) {
            var statusIndex = partyMemberPointer->StatusManager.GetStatusIndex(statusId);
            return partyMemberPointer->StatusManager.GetRemainingTime(statusIndex);
        }

        return 0.0f;
    }
    
    public byte GetLevel() 
        => partyMemberPointer->Level;
    
    public bool HasClassJob(uint classJobId) 
        => partyMemberPointer->ClassJob == classJobId;
    
    public bool IsDead()
        => partyMemberPointer->CurrentHP is 0;
    
    public byte GetClassJob() 
        => partyMemberPointer->ClassJob;
}