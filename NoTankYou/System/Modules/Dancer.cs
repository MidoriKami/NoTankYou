using FFXIVClientStructs.FFXIV.Client.Game.Group;
using NoTankYou.Abstracts;
using NoTankYou.Localization;
using NoTankYou.Models.Enums;
using NoTankYou.Models.Interfaces;
using NoTankYou.Models.ModuleConfiguration;

namespace NoTankYou.System.Modules;

public unsafe class Dancer : ModuleBase
{
    public override ModuleName ModuleName => ModuleName.Dancer;
    public override IModuleConfigBase ModuleConfig { get; protected set; } = new GenericBattleConfiguration();
    protected override string DefaultWarningText { get; } = Strings.DancePartner;

    private const uint DancerClassJobId = 38;
    private const uint MinimumLevel = 60;
    private const int ClosedPositionStatusId = 1823;
    private const int ClosedPositionActionId = 16006;

    protected override bool ShouldEvaluate(IPlayerData playerData)
    {
        if (GroupManager.Instance()->MemberCount < 2) return false;
        if (playerData.MissingClassJob(DancerClassJobId)) return false;
        if (playerData.GetLevel() < MinimumLevel) return false;

        return true;
    }
    
    protected override void EvaluateWarnings(IPlayerData playerData)
    {
        if (playerData.MissingStatus(ClosedPositionStatusId))
        {
            AddActiveWarning(ClosedPositionActionId, playerData);
        }
    }
}