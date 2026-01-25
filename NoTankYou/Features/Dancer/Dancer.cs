using FFXIVClientStructs.FFXIV.Client.Game.Character;
using FFXIVClientStructs.FFXIV.Client.Game.Group;
using NoTankYou.Classes;
using NoTankYou.Enums;

namespace NoTankYou.Features.Dancer;

public unsafe class Dancer : Module<ConfigBase> {

    public override ModuleInfo ModuleInfo => new() {
        DisplayName = "Dancer",
        FileName = "Dancer",
        IconId = 62038,
        Type = ModuleType.ClassFeatures,
    };
    
    private const byte DancerClassJobId = 38;
    private const uint MinimumLevel = 60;
    private const int ClosedPositionStatusId = 1823;
    private const int ClosedPositionActionId = 16006;

    protected override bool ShouldEvaluateWarnings(BattleChara* character) {
        if (GroupManager.Instance()->MainGroup.MemberCount < 2) return false;
        if (character->ClassJob is not DancerClassJobId) return false;
        if (character->Level < MinimumLevel) return false;

        return true;
    }

    protected override void EvaluateWarnings(BattleChara* character) {
        if (character->MissingStatus(ClosedPositionStatusId)) {
            GenerateWarning(ClosedPositionActionId, "Dance Partner", character);
        }
    }
}
