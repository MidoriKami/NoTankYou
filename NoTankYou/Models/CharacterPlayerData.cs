using Dalamud.Memory;
using FFXIVClientStructs.FFXIV.Client.Game.Character;
using NoTankYou.Models.Interfaces;

namespace NoTankYou.Models;

public unsafe class CharacterPlayerData : IPlayerData
{
    private readonly Character* character;
    
    public CharacterPlayerData(Character* characterPointer) => character = characterPointer;
    
    public bool HasStatus(uint statusId) => character->GetStatusManager()->HasStatus(statusId);
    public uint GetObjectId() => character->GameObject.ObjectID;
    public string GetName() => MemoryHelper.ReadStringNullTerminated((nint) character->GameObject.Name);
    public float GetStatusTimeRemaining(uint statusId)
    {
        if (HasStatus(statusId))
        {
            var statusIndex = character->GetStatusManager()->GetStatusIndex(statusId);
            return character->GetStatusManager()->GetRemainingTime(statusIndex);
        }

        return 0.0f;
    }
    public byte GetLevel() => character->CharacterData.Level;
    public byte GetClassJob() => character->CharacterData.ClassJob;
    public bool HasClassJob(uint classJobId) => character->CharacterData.ClassJob == classJobId;
    public bool IsDead() => character->CharacterData.Health is 0;
}