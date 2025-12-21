using System;
using System.Collections.Generic;
using System.Numerics;
using Dalamud.Bindings.ImGui;
using Dalamud.Interface.Utility;
using Dalamud.Interface.Utility.Raii;
using KamiLib.Classes;
using KamiLib.Configuration;
using KamiLib.Extensions;
using NoTankYou.Classes;

namespace NoTankYou.Configuration;

public class PartyListConfig {
    public bool Enabled = true;
    public bool SoloMode;
    public bool SampleMode;
    
    public bool Animation = true;
    public bool UseModuleIcons;
    public bool ShowGlow = true;
    public bool ShowIcon = true;

    public Vector4 GlowColor = new(0.90f, 0.5f, 0.5f, 1.0f);
    
    public HashSet<ModuleName> BlacklistedModules = [];

    public void DrawConfigUi() {
        var configChanged = false;
        
        ImGuiTweaks.Header("Display Options");
        using (var _ = ImRaii.PushIndent()) {
            configChanged |= ImGui.Checkbox("Enable", ref Enabled);
            configChanged |= ImGuiTweaks.Checkbox("Solo Mode", ref SoloMode, "Only show warnings for you");
            configChanged |= ImGui.Checkbox("Sample Mode", ref SampleMode);
        }
        
        ImGuiTweaks.Header("Display Style");
        using (var _ = ImRaii.PushIndent()) {
            configChanged |= ImGui.ColorEdit4("Background Color", ref GlowColor, ImGuiColorEditFlags.AlphaPreviewHalf | ImGuiColorEditFlags.AlphaBar);
            configChanged |= ImGui.Checkbox("Show Background Glow", ref ShowGlow);
            ImGuiHelpers.ScaledDummy(5.0f);
            configChanged |= ImGui.Checkbox("Play Animations", ref Animation);
            configChanged |= ImGui.Checkbox("Show Icon", ref ShowIcon);
            configChanged |= ImGui.Checkbox("Use Module Icon", ref UseModuleIcons);
        }
        
        ImGuiTweaks.Header("Module Blacklist");
        using (var _ = ImRaii.PushIndent()) {
            ImGui.Columns(2);

            foreach (var module in Enum.GetValues<ModuleName>()[..^1]) {
                var inHashset = BlacklistedModules.Contains(module);
                if(ImGui.Checkbox(module.GetDescription(), ref inHashset)) {
                    if (!inHashset) BlacklistedModules.Remove(module);
                    if (inHashset) BlacklistedModules.Add(module);
                    configChanged = true;
                }
                ImGui.NextColumn();
            }
        
            ImGui.Columns();
        }

        if (configChanged) {
            Save();
            System.PartyListController.OnConfigChanged();
        }
    }
    
    public static PartyListConfig Load() 
        => Services.PluginInterface.LoadCharacterFile<PartyListConfig>(Services.PlayerState.ContentId, "PartyListOverlay.config.json");

    public void Save()
        => Services.PluginInterface.SaveCharacterFile(Services.PlayerState.ContentId, "PartyListOverlay.config.json", this);
}