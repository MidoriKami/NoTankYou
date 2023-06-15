using FFXIVClientStructs.FFXIV.Client.Game.Group;
using NoTankYou.Abstracts;
using NoTankYou.Localization;
using NoTankYou.Models.Enums;
using NoTankYou.Models.Interfaces;

namespace NoTankYou.System.Modules;

public unsafe class Dancer : ModuleBase
{
    public override ModuleName ModuleName => ModuleName.Dancer;
    public override string DefaultShortWarning { get; protected set; } = Strings.Dancer_WarningTextShort;
    public override string DefaultLongWarning { get; protected set; } = Strings.Dancer_WarningText;

    private const uint DancerClassJobId = 38;
    private const uint MinimumLevel = 60;
    private const int ClosedPositionStatusId = 1823;
    private const int ClosedPositionActionId = 16006;
    
    protected override void EvaluatePlayer(IPlayerData playerData)
    {
        if (GroupManager.Instance()->MemberCount < 2) return;
        if (playerData.MissingClassJob(DancerClassJobId)) return;
        if (playerData.GetLevel() < MinimumLevel) return;

        if (playerData.MissingStatus(ClosedPositionStatusId))
        {
            AddActiveWarning(ClosedPositionActionId, playerData);
        }
    }
}