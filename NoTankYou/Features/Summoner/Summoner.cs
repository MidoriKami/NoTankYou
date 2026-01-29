using FFXIVClientStructs.FFXIV.Client.Game.Character;
using NoTankYou.Classes;
using NoTankYou.Enums;

namespace NoTankYou.Features.Summoner;

public class Summoner : Module<ConfigBase> {
    public override ModuleInfo ModuleInfo => new() {
        DisplayName = "Summoner",
        FileName = "Summoner",
        IconId = 62027,
        Type = ModuleType.ClassFeatures,
    };

    private const uint SummonCarbuncleActionId = 25798;
    private const byte MinimumLevel = 2;
    private const byte ArcanistJobId = 26;
    private const byte SummonerJobId = 27;

    protected override unsafe bool ShouldEvaluateWarnings(BattleChara* character) {
        if (character->ClassJob is not (SummonerJobId or ArcanistJobId)) return false;
        if (character->Level < MinimumLevel) return false;
        if (!character->GetIsTargetable()) return false;

        return true;
    }

    protected override unsafe void EvaluateWarnings(BattleChara* character) {
        if (character->Pet is null) {
            GenerateWarning(SummonCarbuncleActionId, "Summon Carbuncle", character);
        }
    }
}
