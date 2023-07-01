using System.Linq;
using Dalamud.Game.ClientState.Objects.Types;
using FFXIVClientStructs.FFXIV.Client.Game.Character;
using BattleChara = FFXIVClientStructs.FFXIV.Client.Game.Character.BattleChara;
using Character = FFXIVClientStructs.FFXIV.Client.Game.Character.Character;

namespace NoTankYou.Models.Interfaces;

public unsafe interface IPlayerData
{
    // Interface Members
    bool HasStatus(uint statusId);
    bool HasPet();
    uint GetObjectId();
    byte GetLevel();
    byte GetClassJob();
    bool HasClassJob(uint classJobId);
    bool IsDead();
    string GetName();
    float GetStatusTimeRemaining(uint statusId);

    // Public Helpers
    bool HasStatus(params uint[] statuses) => statuses.Any(HasStatus);
    bool MissingStatus(uint statusID) => !HasStatus(statusID);
    bool MissingStatus(params uint[] statuses) => !statuses.Any(HasStatus);
    bool HasClassJob(params uint[] classJobs) => classJobs.Any(HasClassJob);
    bool MissingClassJob(uint classJobId) => !HasClassJob(classJobId);
    bool MissingClassJob(params uint[] classJobs) => !HasClassJob(classJobs);
    bool GameObjectHasStatus(params uint[] statuses) => statuses.Any(GameObjectHasStatus);
    
    bool GameObjectHasStatus(uint statusId)
    {
        var gameObject = GetCharacterGameObject();
        if (gameObject is null) return false;
        
        return gameObject->GetStatusManager()->HasStatus(statusId);
    }
    
    bool GameObjectHasPet()
    {
        var gameObject = GetCharacterGameObject();
        if (gameObject is null) return true;

        var petChara = CharacterManager.Instance()->LookupPetByOwnerObject((BattleChara*) gameObject);
        return petChara is not null && petChara->Character.CompanionOwnerID == GetObjectId();
    }

    // Internal Helpers
    protected GameObject? GetGameObject() => Service.ObjectTable.SearchById(GetObjectId());
    protected Character* GetCharacterGameObject()
    {
        var gameObject = GetGameObject();
        if (gameObject is null) return null;
        
        return (Character*) gameObject.Address;
    }
}