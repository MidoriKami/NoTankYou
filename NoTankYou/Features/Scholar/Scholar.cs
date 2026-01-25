using FFXIVClientStructs.FFXIV.Client.Game.Character;
using NoTankYou.Classes;
using NoTankYou.Enums;

namespace NoTankYou.Features.Scholar;

public unsafe class Sccholar : Module<ConfigBase> {
    public override ModuleInfo ModuleInfo => new() {
        DisplayName = "Scholar",
        FileName = "Scholar",
        IconId = 62028,
        Type = ModuleType.ClassFeatures,
    };

    private const int DissipationStatusId = 791;
    private const byte ScholarJobId = 28;
    private const uint SummonEosActionId = 17215;
    private const int MinimumLevel = 4;

    private readonly Debouncer dissipationDebouncer = new();

    protected override bool ShouldEvaluateWarnings(BattleChara* character) {
        if (character->ClassJob is not ScholarJobId) return false;
        if (character->Level < MinimumLevel) return false;
        if (!character->GetIsTargetable()) return false;

        return true;
    }

    protected override void EvaluateWarnings(BattleChara* character) {
        var hasDissipation = character->HasStatus(DissipationStatusId);
        
        dissipationDebouncer.Update(character->EntityId, hasDissipation);
        if (dissipationDebouncer.IsLockedOut(character->EntityId)) return;

        if (!hasDissipation && character->Pet is null) {
            GenerateWarning(SummonEosActionId, "Summon Faerie", character);
        }
    }
}
