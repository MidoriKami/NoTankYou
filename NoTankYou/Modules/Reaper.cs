using System;
using Dalamud.Bindings.ImGui;
using Dalamud.Interface.Utility;
using KamiLib.Extensions;
using NoTankYou.Classes;
using NoTankYou.PlayerDataInterface;

namespace NoTankYou.Modules;

public class Reaper : ModuleBase<ReaperConfiguration> {
    public override ModuleName ModuleName => ModuleName.Reaper;

    protected override string DefaultWarningText => "Status Missing";
    
    private const uint ReaperClassJobId = 39;
    private const uint MinimumLevel = 82;

    private const uint SoulsowActionId = 24387;
    private const uint SoulsowStatusId = 2594;
    
    private DateTime lastCombatTime = DateTime.UtcNow;
    
    protected override bool ShouldEvaluate(IPlayerData playerData) {
        if (Services.ObjectTable.LocalPlayer?.EntityId != playerData.GetEntityId()) return false;
        if (playerData.GetClassJob() != ReaperClassJobId) return false;
        if (playerData.GetLevel() < MinimumLevel) return false;

        return true;
    }

    protected override void EvaluateWarnings(IPlayerData playerData) {
        if (Services.Condition.IsInCombat()) {
            lastCombatTime = DateTime.UtcNow;
        }

        if (DateTime.UtcNow - lastCombatTime > TimeSpan.FromSeconds(Config.WarningDelay)) {
            if (playerData.MissingStatus(SoulsowStatusId)) {
                AddActiveWarning(SoulsowActionId, playerData);
            }
        }
    }
}

public class ReaperConfiguration() : ModuleConfigBase(ModuleName.Reaper) {
    public override OptionDisableFlags OptionDisableFlags => OptionDisableFlags.SoloMode;
    
    public int WarningDelay = 5;

    protected override void DrawModuleConfig() {
        ImGui.PushItemWidth(50.0f * ImGuiHelpers.GlobalScale);
        ConfigChanged |= ImGui.InputInt("Warning Delay Time (seconds)", ref WarningDelay);
    }
}