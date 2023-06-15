using FFXIVClientStructs.FFXIV.Client.Game.Character;
using NoTankYou.Models.Interfaces;

namespace NoTankYou.Models;

public unsafe class CharacterPlayerData : IPlayerData
{
    private readonly Character* character;
    
    public CharacterPlayerData(Character* characterPointer)
    {
        character = characterPointer;
    }

    public bool HasStatus(uint statusId) => character->GetStatusManager()->HasStatus(statusId);
    public bool HasPet() => CharacterManager.Instance()->LookupPetByOwnerObject((BattleChara*) &character->GameObject) != null;
    public uint GetObjectId() => character->GameObject.ObjectID;
    public byte* GetName() => character->GameObject.Name;
    public byte GetLevel() => character->Level;
    public byte GetClassJob() => character->ClassJob;
    public bool HasClassJob(uint classJobId) => character->ClassJob == classJobId;
}