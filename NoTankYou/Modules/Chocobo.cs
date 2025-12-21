using Dalamud.Bindings.ImGui;
using Dalamud.Interface.Utility;
using FFXIVClientStructs.FFXIV.Client.Game.UI;
using KamiLib.Extensions;
using Lumina.Excel.Sheets;
using NoTankYou.Classes;
using NoTankYou.PlayerDataInterface;

namespace NoTankYou.Modules;

public unsafe class Chocobo : ModuleBase<ChocoboConfiguration> {
    public override ModuleName ModuleName => ModuleName.Chocobo;
    protected override string DefaultWarningText => "Chocobo Missing";

    private static Item GyshalGreensItem => Services.DataManager.GetExcelSheet<Item>().GetRow(GyshalGreensItemId);

    private const uint GyshalGreensItemId = 4868;
    private readonly uint gyshalGreensIconId = GyshalGreensItem.Icon;
    private readonly string gyshalGreensActionName = GyshalGreensItem.Name.ToString();

    protected override bool ShouldEvaluate(IPlayerData playerData) {
        if (TerritoryInfo.Instance()->InSanctuary) return false;
        if (Services.Condition.IsBoundByDuty()) return false;
        if (Config.DisableInCombat && Services.Condition.IsInCombat()) return false;
        if (playerData.GetEntityId() != Services.ObjectTable.LocalPlayer?.EntityId) return false;

        return true;
    }
    
    protected override void EvaluateWarnings(IPlayerData playerData) {
        var warningTime = Config.EarlyWarning ? Config.EarlyWarningTime : 0;

        if (UIState.Instance()->Buddy.CompanionInfo.TimeLeft <= warningTime) {
            AddActiveWarning(gyshalGreensIconId, gyshalGreensActionName, playerData);
        }
    }
}

public class ChocoboConfiguration() : ModuleConfigBase(ModuleName.Chocobo) {
    public override OptionDisableFlags OptionDisableFlags => OptionDisableFlags.Sanctuary | OptionDisableFlags.DutiesOnly | OptionDisableFlags.SoloMode;

    public bool DisableInCombat = true;
    public bool EarlyWarning = true;
    public int EarlyWarningTime = 300;

    protected override void DrawModuleConfig() {
        ConfigChanged |= ImGui.Checkbox("Suppress in Combat", ref DisableInCombat);
        ConfigChanged |= ImGui.Checkbox("Early Warning", ref EarlyWarning);
        
        ImGui.PushItemWidth(50.0f * ImGuiHelpers.GlobalScale);
        ConfigChanged |= ImGui.InputInt("Early Warning Time", ref EarlyWarningTime);
    }
}