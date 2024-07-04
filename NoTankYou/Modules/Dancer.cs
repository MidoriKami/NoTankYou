using FFXIVClientStructs.FFXIV.Client.Game.Group;
using NoTankYou.Classes;
using NoTankYou.Localization;
using NoTankYou.PlayerDataInterface;

namespace NoTankYou.Modules;

public unsafe class Dancer : ModuleBase<DancerConfiguration> {
    public override ModuleName ModuleName => ModuleName.Dancer;
    protected override string DefaultWarningText => Strings.DancePartner;

    private const uint DancerClassJobId = 38;
    private const uint MinimumLevel = 60;
    private const int ClosedPositionStatusId = 1823;
    private const int ClosedPositionActionId = 16006;

    protected override bool ShouldEvaluate(IPlayerData playerData) {
        if (GroupManager.Instance()->MainGroup.MemberCount < 2) return false;
        if (playerData.MissingClassJob(DancerClassJobId)) return false;
        if (playerData.GetLevel() < MinimumLevel) return false;

        return true;
    }
    
    protected override void EvaluateWarnings(IPlayerData playerData) {
        if (playerData.MissingStatus(ClosedPositionStatusId)) {
            AddActiveWarning(ClosedPositionActionId, playerData);
        }
    }
}

public class DancerConfiguration() : ModuleConfigBase(ModuleName.Dancer);
