using System;
using System.Collections.Generic;
using System.Numerics;
using Dalamud.Interface.Utility;
using Dalamud.Interface.Utility.Raii;
using ImGuiNET;
using KamiLib.Classes;
using KamiLib.Configuration;
using KamiLib.Extensions;
using NoTankYou.Classes;
using NoTankYou.Localization;

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
        
        ImGuiTweaks.Header(Strings.DisplayOptions);
        using (var _ = ImRaii.PushIndent()) {
            configChanged |= ImGui.Checkbox(Strings.Enable, ref Enabled);
            configChanged |= ImGuiTweaks.Checkbox(Strings.SoloMode, ref SoloMode, Strings.SoloModeHelp);
            configChanged |= ImGui.Checkbox(Strings.SampleMode, ref SampleMode);
        }
        
        ImGuiTweaks.Header(Strings.DisplayStyle);
        using (var _ = ImRaii.PushIndent()) {
            configChanged |= ImGui.ColorEdit4("Background Color", ref GlowColor, ImGuiColorEditFlags.AlphaPreviewHalf | ImGuiColorEditFlags.AlphaBar);
            configChanged |= ImGui.Checkbox("Show Background Glow", ref ShowGlow);
            ImGuiHelpers.ScaledDummy(5.0f);
            configChanged |= ImGui.Checkbox("Play Animations", ref Animation);
            configChanged |= ImGui.Checkbox("Show Icon", ref ShowIcon);
            configChanged |= ImGui.Checkbox("Use Module Icon", ref UseModuleIcons);
        }
        
        ImGuiTweaks.Header(Strings.ModuleBlacklist);
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
        
            ImGui.Columns(1);
        }

        if (configChanged) {
            Save();
            System.PartyListController.OnConfigChanged();
        }
    }
    
    public static PartyListConfig Load() 
        => Service.PluginInterface.LoadCharacterFile(Service.ClientState.LocalContentId, "PartyListOverlay.config.json", () => new PartyListConfig());

    public void Save()
        => Service.PluginInterface.SaveCharacterFile(Service.ClientState.LocalContentId, "PartyListOverlay.config.json", this);
}