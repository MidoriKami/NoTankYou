using NoTankYou.Abstracts;
using NoTankYou.Localization;
using NoTankYou.Models;
using NoTankYou.Models.Enums;
using NoTankYou.Models.Interfaces;
using NoTankYou.Models.ModuleConfiguration;

namespace NoTankYou.System.Modules;

public class Scholar : ModuleBase
{
    public override ModuleName ModuleName => ModuleName.Scholar;
    public override string DefaultWarningText { get; protected set; } = Strings.ScholarFaerie;
    public override IModuleConfigBase ModuleConfig { get; protected set; } = new GenericBattleConfiguration();

    private const int DissipationStatusId = 791;
    private const byte ScholarJobId = 28;
    private const uint SummonEosActionId = 17215;
    private const int MinimumLevel = 4;

    private readonly Debouncer dissipationDebouncer = new();
    
    protected override bool ShouldEvaluate(IPlayerData playerData)
    {
        if (playerData.MissingClassJob(ScholarJobId)) return false;
        if(playerData.GetLevel() < MinimumLevel) return false;
        
        return true;
    }
    
    protected override void EvaluateWarnings(IPlayerData playerData)
    {
        var hasDissipation = playerData.HasStatus(DissipationStatusId);
        
        dissipationDebouncer.Update(playerData.GetObjectId(), hasDissipation);
        if (dissipationDebouncer.IsLockedOut(playerData.GetObjectId())) return;

        if (!hasDissipation && !playerData.HasPet())
        {
            AddActiveWarning(SummonEosActionId, playerData);
        }
    }
}