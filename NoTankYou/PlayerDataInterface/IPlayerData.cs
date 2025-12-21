using System.Linq;
using Dalamud.Game.ClientState.Objects.Enums;
using Dalamud.Game.ClientState.Objects.Types;
using Character = FFXIVClientStructs.FFXIV.Client.Game.Character.Character;

namespace NoTankYou.PlayerDataInterface;

public unsafe interface IPlayerData {
    // Interface Members
    bool HasStatus(uint statusId);
    
    uint GetEntityId();
    
    byte GetLevel();
    
    byte GetClassJob();
    
    bool HasClassJob(uint classJobId);
    
    bool IsDead();
    
    string GetName();
    
    float GetStatusTimeRemaining(uint statusId);

    // Public Helpers
    bool HasStatus(params uint[] statuses) 
        => statuses.Any(HasStatus);
    
    bool MissingStatus(uint statusId)
        => !HasStatus(statusId);
    
    bool MissingStatus(params uint[] statuses) 
        => !statuses.Any(HasStatus);
    
    bool HasClassJob(params uint[] classJobs) 
        => classJobs.Any(HasClassJob);
    
    bool MissingClassJob(uint classJobId)
        => !HasClassJob(classJobId);
    
    bool MissingClassJob(params uint[] classJobs) 
        => !HasClassJob(classJobs);
    
    bool GameObjectHasStatus(params uint[] statuses) 
        => statuses.Any(GameObjectHasStatus);
    
    bool IsTargetable() 
        => GetGameObject()?.IsTargetable ?? false;
    
    bool GameObjectHasStatus(uint statusId) {
        var gameObject = GetCharacterGameObject();
        if (gameObject is null) return false;
        
        return gameObject->GetStatusManager()->HasStatus(statusId);
    }
    
    bool HasPet()
        => Services.ObjectTable
            .Where(obj => obj.OwnerId == GetEntityId())
            .Where(obj => obj is IBattleNpc { SubKind: (byte) BattleNpcSubKind.Pet })
            .Any();
    
    // Internal Helpers
    protected IGameObject? GetGameObject() 
        => Services.ObjectTable.SearchById(GetEntityId());
    
    protected Character* GetCharacterGameObject() {
        var gameObject = GetGameObject();
        if (gameObject is null) return null;
        
        return (Character*) gameObject.Address;
    }
}