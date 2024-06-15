using NoTankYou.Classes;
using NoTankYou.Localization;
using NoTankYou.PlayerDataInterface;

namespace NoTankYou.Modules;

public class Summoner : ModuleBase<SummonerConfiguration> {
    public override ModuleName ModuleName => ModuleName.Summoner;
    protected override string DefaultWarningText => Strings.SummonerPet;

    private const uint SummonCarbuncleActionId = 25798;
    private const byte MinimumLevel = 2;
    private const byte ArcanistJobId = 26;
    private const byte SummonerJobId = 27;

    private readonly Debouncer petDebouncer = new();
    
    protected override bool ShouldEvaluate(IPlayerData playerData) {
        if (playerData.MissingClassJob(ArcanistJobId, SummonerJobId)) return false;
        if (playerData.GetLevel() < MinimumLevel) return false;
        if (!playerData.IsTargetable()) return false;

        return true;
    }
    
    protected override void EvaluateWarnings(IPlayerData playerData) {
        petDebouncer.Update(playerData.GetEntityId(), playerData.HasPet());
        if (petDebouncer.IsLockedOut(playerData.GetEntityId())) return;
        
        if (!playerData.HasPet()) {
            AddActiveWarning(SummonCarbuncleActionId, playerData);
        }
    }
}

public class SummonerConfiguration() : ModuleConfigBase(ModuleName.Summoner);
