using FFXIVClientStructs.FFXIV.Client.Game.Group;
using FFXIVClientStructs.Interop;
using NoTankYou.Classes;
using NoTankYou.Localization;
using NoTankYou.PlayerDataInterface;

namespace NoTankYou.Modules;

public unsafe class BlueMage : ModuleBase<BlueMageConfiguration> {
    public override ModuleName ModuleName => ModuleName.BlueMage;
    protected override string DefaultWarningText => Strings.MightyGuard;

    private const uint MightyGuardStatusId = 1719;
    private const uint AetherialMimicryTankStatusId = 2124;
    private const uint MightyGuardActionId = 11417;
    private const byte BlueMageClassJob = 36;
    
    protected override bool ShouldEvaluate(IPlayerData playerData) {
        if (!playerData.HasClassJob(BlueMageClassJob)) return false;
        if (playerData.MissingStatus(AetherialMimicryTankStatusId)) return false;

        return true;
    }
    
    protected override void EvaluateWarnings(IPlayerData playerData) {
        if (GroupManager.Instance()->MainGroup.MemberCount is 0) {
            if (playerData.MissingStatus(MightyGuardStatusId)) {
                AddActiveWarning(MightyGuardActionId, playerData);
            }
        }
        else {
            if (!PartyHasStance()) {
                AddActiveWarning(MightyGuardActionId, playerData);
            }
        }
    }

    private static bool PartyHasStance() {
        foreach (var partyMember in PartyMemberSpan.PointerEnumerator()) {
            IPlayerData playerData = new PartyMemberPlayerData(partyMember);

            if (playerData.MissingStatus(AetherialMimicryTankStatusId)) continue;
            if (playerData.HasStatus(MightyGuardStatusId)) return true;
        }

        return false;
    }
}

public class BlueMageConfiguration() : ModuleConfigBase(ModuleName.BlueMage);
