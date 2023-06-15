using FFXIVClientStructs.FFXIV.Client.Game.Character;
using FFXIVClientStructs.FFXIV.Client.Game.Group;
using NoTankYou.Models.Interfaces;

namespace NoTankYou.Models;

public unsafe class PartyMemberPlayerData : IPlayerData
{
    private readonly PartyMember* partyMember;

    public PartyMemberPlayerData(PartyMember* partyMemberPointer)
    {
        partyMember = partyMemberPointer;
    }

    public bool HasStatus(uint statusId) => partyMember->StatusManager.HasStatus(statusId);
    public uint GetObjectId() => partyMember->ObjectID;
    public byte* GetName() => partyMember->Name;
    public byte GetLevel() => partyMember->Level;
    public bool HasClassJob(uint classJobId) => partyMember->ClassJob == classJobId;
    public byte GetClassJob() => partyMember->ClassJob;

    public bool HasPet()
    {
        var gameObject = Service.ObjectTable.SearchById(partyMember->ObjectID);
        if (gameObject is null) return true; // If we can't find the object, then assume they have a pet
        
        return CharacterManager.Instance()->LookupPetByOwnerObject((BattleChara*) gameObject.Address) != null;
    }
}