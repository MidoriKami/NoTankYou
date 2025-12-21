using System;
using System.Drawing;
using Dalamud.Bindings.ImGui;
using Dalamud.Interface;
using Dalamud.Interface.Utility;
using Dalamud.Interface.Utility.Raii;
using KamiLib.Classes;
using KamiLib.Extensions;
using NoTankYou.Classes;

namespace NoTankYou.Windows;


public class BannerSettingsTab : ITabItem {
	public string Name => "Banner";
    
	public bool Disabled => false;

    public void Draw() {
        DrawGeneralSettings();
        DrawBannerListSettings();
        DrawBannerNodeSettings();
        DrawModuleBlacklist();
    }

    private static void DrawGeneralSettings() {
        var config = System.BannerConfig;
        if (config is null) {
            ImGui.TextColored(KnownColor.Orange.Vector(), "Banner Settings Failed to Load.");
            return;
        }
        
        var configChanged = false;
        
        ImGuiTweaks.Header("Display Options");
        using (var _ = ImRaii.PushIndent()) {
            configChanged |= ImGui.Checkbox("Enable", ref config.Enabled);
            configChanged |= ImGuiTweaks.Checkbox("Solo Mode", ref config.SoloMode, "Only show warnings for you");
            configChanged |= ImGui.Checkbox("Sample Mode", ref config.SampleMode);
            configChanged |= ImGuiTweaks.EnumCombo("Display Mode", ref config.DisplayMode);
        }

        if (configChanged) {
            Utilities.Config.SaveCharacterConfig(config, "System.config.json");
        }
    }

    private static void DrawBannerListSettings() {
        var config = System.BannerListStyle;
        if (config is null) {
            ImGui.TextColored(KnownColor.Orange.Vector(), "Banner List Style Failed to Load.");
            return;
        }

        var configChanged = false;

        ImGuiTweaks.Header("Positioning");
        using (var _ = ImRaii.PushIndent()) {
            configChanged |= ImGui.Checkbox("Enable Moving", ref config.EnableMoving);
            configChanged |= ImGui.Checkbox("Enable Resizing", ref config.EnableResizing);
            configChanged |= ImGui.DragFloat2("Scale", ref config.Scale, 0.05f, 0.1f, 6.0f);
        }
        
        ImGuiTweaks.Header("Display Mode");
        using (var _ = ImRaii.PushIndent()) {
            
            configChanged |= ImGuiTweaks.EnumCombo("Layout Orientation", ref config.Orientation);
            configChanged |= ImGuiTweaks.EnumCombo("Layout Anchor", ref config.Anchor);
            
            ImGuiHelpers.ScaledDummy(5.0f);

            configChanged |= ImGui.Checkbox("Show Background", ref config.ShowBackground);
            configChanged |= ImGuiTweaks.ColorEditWithDefault("Background Color", ref config.BackgroundColor, KnownColor.Aqua.Vector() with { W = 0.15f });
        }

        if (configChanged) {
            Utilities.Config.SaveCharacterConfig(config, "BannerList.style.json");
        }
	}

    private static void DrawBannerNodeSettings() {
        var config = System.BannerStyle;
        if (config is null) {
            ImGui.TextColored(KnownColor.Orange.Vector(), "Individual Banner Style Failed to Load.");
            return;
        }

        var configChanged = false;
        
        ImGuiTweaks.Header("Display Style");
        using (var _ = ImRaii.PushIndent()) {
            configChanged |= ImGui.Checkbox("Warning Shield", ref config.ShowWarningIcon);
            configChanged |= ImGui.Checkbox("Warning Text", ref config.ShowMessageText);
            configChanged |= ImGui.Checkbox("Player Name", ref config.ShowPlayerText);
            configChanged |= ImGui.Checkbox("Action Name", ref config.ShowActionName);
            configChanged |= ImGui.Checkbox("Action Icon", ref config.ShowActionIcon);
            configChanged |= ImGui.Checkbox("Play Animations", ref config.EnableAnimation);
            configChanged |= ImGui.Checkbox("Show Action Tooltip", ref config.EnableActionTooltip);

            ImGuiHelpers.ScaledDummy(5.0f);
        }

        if (configChanged) {
            Utilities.Config.SaveCharacterConfig(config, "BannerNode.style.json");
        }
    }
    
    private static void DrawModuleBlacklist() {
        var config = System.BannerConfig;
        if (config is null) {
            ImGui.TextColored(KnownColor.Orange.Vector(), "Banner Blacklist Failed to Load.");
            return;
        }

        var configChanged = false;
        
        ImGuiTweaks.Header("Module Blacklist");
        using (var _ = ImRaii.PushIndent()) {
            ImGui.Columns(2);
        
            foreach (var module in Enum.GetValues<ModuleName>()[..^1]) {
                var inHashset = config.BlacklistedModules.Contains(module);
                if(ImGui.Checkbox(module.GetDescription(), ref inHashset)) {
                    if (!inHashset) config.BlacklistedModules.Remove(module);
                    if (inHashset) config.BlacklistedModules.Add(module);
                    configChanged = true;
                }
                ImGui.NextColumn();
            }
        
            ImGui.Columns();
        }

        if (configChanged) {
            Utilities.Config.SaveCharacterConfig(config, "System.config.json");
        }
    }
}