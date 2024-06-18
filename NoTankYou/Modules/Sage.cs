using FFXIVClientStructs.FFXIV.Client.Game.Group;
using ImGuiNET;
using NoTankYou.Classes;
using NoTankYou.Localization;
using NoTankYou.PlayerDataInterface;

namespace NoTankYou.Modules;

public class Sage : ModuleBase<SageConfiguration> {
    public override ModuleName ModuleName => ModuleName.Sage;
    protected override string DefaultWarningText { get; } = Strings.SageKardion;

    private const byte MinimumLevel = 4;
    private const byte SageClassJob = 40;
    private const int KardiaStatusId = 2604;
    private const int KardiaActionId = 24285;
    
    protected override unsafe bool ShouldEvaluate(IPlayerData playerData) {
        if (Config.DisableWhileSolo && GroupManager.Instance()->MemberCount is 0) return false;
        if (playerData.MissingClassJob(SageClassJob)) return false;
        if (playerData.GetLevel() < MinimumLevel) return false;

        return true;
    }
    
    protected override void EvaluateWarnings(IPlayerData playerData) {
        if (playerData.MissingStatus(KardiaStatusId)) {
            AddActiveWarning(KardiaActionId, playerData);
        }
    }
}

public class SageConfiguration() : ModuleConfigBase(ModuleName.Sage) {
    public bool DisableWhileSolo = true;
    
    protected override void DrawModuleConfig() {
        ConfigChanged = ImGui.Checkbox("Disable while Solo", ref DisableWhileSolo);
    }
};
