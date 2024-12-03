using System.Collections.Generic;
using Dalamud.Interface.Utility;
using Dalamud.Interface.Utility.Raii;
using FFXIVClientStructs.FFXIV.Client.Game;
using ImGuiNET;
using KamiLib.Extensions;
using Lumina.Excel.Sheets;
using NoTankYou.Localization;
using NoTankYou.PlayerDataInterface;

namespace NoTankYou.Classes;

public abstract class ConsumableModule<T> : ModuleBase<T> where T : ConsumableConfiguration, new() {
    protected abstract uint IconId { get; }
    protected abstract string IconLabel { get; }
    protected abstract uint StatusId { get; }
    
    protected override unsafe bool ShouldEvaluate(IPlayerData playerData) {
        if (Config.SuppressInCombat && Service.Condition.IsInCombat()) return false;

        if (Config.SavageFilter || Config.UltimateFilter || Config.ExtremeUnrealFilter || Config.CriterionFilter) {
            var allowedZones = new List<DutyType>();
            
            if(Config.SavageFilter) allowedZones.Add(DutyType.Savage);
            if(Config.UltimateFilter) allowedZones.Add(DutyType.Ultimate);
            if(Config.ExtremeUnrealFilter) allowedZones.Add(DutyType.Extreme);
            if(Config.ExtremeUnrealFilter) allowedZones.Add(DutyType.Unreal);
            if(Config.CriterionFilter) allowedZones.Add(DutyType.Criterion);

            var currentCfc = Service.DataManager.GetExcelSheet<ContentFinderCondition>().GetRow(GameMain.Instance()->CurrentContentFinderConditionId);
            if (currentCfc.RowId is 0) return false;

            var currentDutyType = Service.DataManager.GetDutyType(currentCfc);
            if (!allowedZones.Contains(currentDutyType)) return false;
        }
        
        return true;
    }
    
    protected override void EvaluateWarnings(IPlayerData playerData) {
        var statusTimeRemaining = playerData.GetStatusTimeRemaining(StatusId);
        
        if (statusTimeRemaining < Config.EarlyWarningTime) {
            if (Config is { ShowTimeRemaining: true } && statusTimeRemaining is not 0) {
                ExtraWarningText = $" ({(int)playerData.GetStatusTimeRemaining(StatusId)}s)";
            }
            else {
                ExtraWarningText = string.Empty;
            }
            
            AddActiveWarning(IconId, IconLabel, playerData);
        }
    }
}

public abstract class ConsumableConfiguration(ModuleName moduleName) : ModuleConfigBase(moduleName) {
    public bool SuppressInCombat = true;
    public int EarlyWarningTime = 600;
    public bool ShowTimeRemaining;
    
    public bool SavageFilter;
    public bool UltimateFilter;
    public bool ExtremeUnrealFilter;
    public bool CriterionFilter;

    protected override void DrawModuleConfig() {
        ConfigChanged |= ImGui.Checkbox(Strings.SuppressInCombat, ref SuppressInCombat);
        
        ImGui.PushItemWidth(50.0f * ImGuiHelpers.GlobalScale);
        ConfigChanged |= ImGui.InputInt(Strings.EarlyWarningTime, ref EarlyWarningTime, 0, 0);
        ConfigChanged |= ImGui.Checkbox(Strings.ShowTimeRemaining, ref ShowTimeRemaining);
        
        ImGuiHelpers.ScaledDummy(10.0f);

        using (var _ = ImRaii.PushIndent(-1)) {
            ImGui.Text(Strings.ZoneFilter);
            ImGui.Separator();
        }
        
        ImGuiHelpers.ScaledDummy(5.0f);
        ConfigChanged |= ImGui.Checkbox(Strings.Savage, ref SavageFilter);
        ConfigChanged |= ImGui.Checkbox(Strings.Ultimate, ref UltimateFilter);
        ConfigChanged |= ImGui.Checkbox(Strings.ExtremeUnreal, ref ExtremeUnrealFilter);
        ConfigChanged |= ImGui.Checkbox(Strings.Criterion, ref CriterionFilter);
    }
}