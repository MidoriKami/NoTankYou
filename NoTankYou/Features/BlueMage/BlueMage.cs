using System.Linq;
using FFXIVClientStructs.FFXIV.Client.Game.Character;
using FFXIVClientStructs.FFXIV.Client.Game.Group;
using NoTankYou.Classes;
using NoTankYou.Enums;

namespace NoTankYou.Features.BlueMage;

public unsafe class BlueMage : Module<ConfigBase> {
    public override ModuleInfo ModuleInfo => new() {
        DisplayName = "Blue Mage",
        FileName = "BlueMage",
        IconId = 62036,
        Type = ModuleType.ClassFeatures,
    };

    private const uint MightyGuardStatusId = 1719;
    private const uint AetherialMimicryTankStatusId = 2124;
    private const uint MightyGuardActionId = 11417;
    private const byte BlueMageClassJob = 36;
    
    protected override bool ShouldEvaluateWarnings(BattleChara* character) {
        if (character->ClassJob is not BlueMageClassJob) return false;
        if (character->MissingStatus(AetherialMimicryTankStatusId)) return false;

        return true;
    }

    protected override void EvaluateWarnings(BattleChara* character) {
        if (GroupManager.Instance()->MainGroup.MemberCount is 0) {
            if (character->MissingStatus(MightyGuardStatusId)) {
                GenerateWarning(MightyGuardActionId, "Mighty Guard", character);
            }
        }
        else {
            // This is probably wrong, will wait for someone to complain first
            if (PartyMembers.Any(member => member.Value->HasStatus(AetherialMimicryTankStatusId) && member.Value->HasStatus(MightyGuardStatusId))) {
                GenerateWarning(MightyGuardActionId, "Mighty Guard", character);
            }
        }
    }
}
