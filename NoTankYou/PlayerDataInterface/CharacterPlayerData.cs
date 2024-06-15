using FFXIVClientStructs.FFXIV.Client.Game.Character;

namespace NoTankYou.PlayerDataInterface;

public unsafe class CharacterPlayerData(Character* characterPointer) : IPlayerData {
    public bool HasStatus(uint statusId) 
        => characterPointer->GetStatusManager()->HasStatus(statusId);
    
    public uint GetEntityId() 
        => characterPointer->GameObject.EntityId;
    
    public string GetName() 
        => characterPointer->NameString;
    
    public float GetStatusTimeRemaining(uint statusId) {
        if (HasStatus(statusId)) {
            var statusIndex = characterPointer->GetStatusManager()->GetStatusIndex(statusId);
            return characterPointer->GetStatusManager()->GetRemainingTime(statusIndex);
        }

        return 0.0f;
    }
    
    public byte GetLevel() 
        => characterPointer->CharacterData.Level;
    
    public byte GetClassJob() 
        => characterPointer->CharacterData.ClassJob;
    
    public bool HasClassJob(uint classJobId)
        => characterPointer->CharacterData.ClassJob == classJobId;
    
    public bool IsDead() 
        => characterPointer->CharacterData.Health is 0;
}