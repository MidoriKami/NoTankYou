using System;
using FFXIVClientStructs.FFXIV.Client.Game.Character;
using NoTankYou.Classes;
using NoTankYou.Enums;

namespace NoTankYou.Features.Reaper;

public class Reaper : Module<ReaperConfig> {
    public override ModuleInfo ModuleInfo => new() {
        DisplayName = "Reaper",
        FileName = "Reaper",
        IconId = 62039,
        Type = ModuleType.ClassFeatures,
    };

    private const byte ReaperClassJobId = 39;
    private const uint MinimumLevel = 82;

    private const uint SoulsowActionId = 24387;
    private const uint SoulsowStatusId = 2594;
    
    private DateTime lastCombatTime = DateTime.UtcNow;
    
    protected override unsafe bool ShouldEvaluateWarnings(BattleChara* character) {
        if (character->ObjectIndex is not 0) return false;
        if (character->ClassJob is not ReaperClassJobId) return false;
        if (character->Level < MinimumLevel) return false;

        return true;
    }

    protected override unsafe void EvaluateWarnings(BattleChara* character) {
        if (Services.Condition.IsInCombat) lastCombatTime = DateTime.UtcNow;

        if (DateTime.UtcNow - lastCombatTime <= TimeSpan.FromSeconds(ModuleConfig.WarningDelay)) return;

        if (character->MissingStatus(SoulsowStatusId)) {
            GenerateWarning(SoulsowActionId, "Soulsow", character);
        }
    }
}
