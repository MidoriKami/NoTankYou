using NoTankYou.Classes;
using NoTankYou.Localization;
using NoTankYou.PlayerDataInterface;

namespace NoTankYou.Modules;

public class Scholar : ModuleBase<ScholarConfiguration> {
    public override ModuleName ModuleName => ModuleName.Scholar;
    protected override string DefaultWarningText { get; } = Strings.ScholarFaerie;

    private const int DissipationStatusId = 791;
    private const byte ScholarJobId = 28;
    private const uint SummonEosActionId = 17215;
    private const int MinimumLevel = 4;

    private readonly Debouncer dissipationDebouncer = new();
    
    protected override bool ShouldEvaluate(IPlayerData playerData) {
        if (playerData.MissingClassJob(ScholarJobId)) return false;
        if (playerData.GetLevel() < MinimumLevel) return false;
        if (!playerData.IsTargetable()) return false;
        
        return true;
    }
    
    protected override void EvaluateWarnings(IPlayerData playerData) {
        var hasDissipation = playerData.HasStatus(DissipationStatusId);
        
        dissipationDebouncer.Update(playerData.GetEntityId(), hasDissipation);
        if (dissipationDebouncer.IsLockedOut(playerData.GetEntityId())) return;

        if (!hasDissipation && !playerData.HasPet()) {
            AddActiveWarning(SummonEosActionId, playerData);
        }
    }
}

public class ScholarConfiguration() : ModuleConfigBase(ModuleName.Scholar);
