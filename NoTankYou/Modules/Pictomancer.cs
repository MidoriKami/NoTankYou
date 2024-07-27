using System;
using Dalamud.Game.ClientState.JobGauge.Types;
using Dalamud.Interface.Utility;
using ImGuiNET;
using KamiLib.Extensions;
using NoTankYou.Classes;
using NoTankYou.PlayerDataInterface;

namespace NoTankYou.Modules;

public class Pictomancer : ModuleBase<PictomancerConfiguration> {
    public override ModuleName ModuleName => ModuleName.Pictomancer;
    protected override string DefaultWarningText => "Missing Motif";
    
    private const uint PictoClassJobId = 42;
    private const uint MinimumLevel = 30;

    private const uint CreatureMinimumLevel = 30;
    private const uint CreatureActionId = 34664;

    private const uint WeaponMinimumLevel = 50;
    private const uint WeaponActionId = 34668;

    private const uint LandscapeMinimumLevel = 70;
    private const uint LandscapeActionId = 34669;

    private DateTime lastCombatTime = DateTime.UtcNow;
    
    protected override bool ShouldEvaluate(IPlayerData playerData) {
        if (playerData.GetClassJob() != PictoClassJobId) return false;
        if (playerData.GetLevel() < MinimumLevel) return false;

        return true;
    }

    protected override void EvaluateWarnings(IPlayerData playerData) {
        if (Service.Condition.IsInCombat()) {
            lastCombatTime = DateTime.UtcNow;
        }

        if (DateTime.UtcNow - lastCombatTime > TimeSpan.FromSeconds(Config.WarningDelay)) {
            if (playerData.GetLevel() >= CreatureMinimumLevel && !Service.JobGauges.Get<PCTGauge>().CreatureMotifDrawn) {
                AddActiveWarning(CreatureActionId, playerData);
                return;
            }
        
            if (playerData.GetLevel() >= WeaponMinimumLevel && !Service.JobGauges.Get<PCTGauge>().WeaponMotifDrawn) {
                AddActiveWarning(WeaponActionId, playerData);
                return;
            }
        
            if (playerData.GetLevel() >= LandscapeMinimumLevel && !Service.JobGauges.Get<PCTGauge>().LandscapeMotifDrawn) {
                AddActiveWarning(LandscapeActionId, playerData);
                // return; // Redundant for now?
            }
        }
    }
}

public class PictomancerConfiguration() : ModuleConfigBase(ModuleName.Pictomancer) {
    protected override OptionDisableFlags OptionDisableFlags => OptionDisableFlags.SoloMode;
    
    public int WarningDelay = 5;

    protected override void DrawModuleConfig() {
        ImGui.PushItemWidth(50.0f * ImGuiHelpers.GlobalScale);
        ConfigChanged |= ImGui.InputInt("Warning Delay Time (seconds)", ref WarningDelay, 0, 0);
    }
}
