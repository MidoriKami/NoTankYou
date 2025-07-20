using System;
using System.Collections.Generic;
using System.Drawing;
using System.Numerics;
using Dalamud.Interface;
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
    
    public Vector4 OutlineColor  = KnownColor.Red.Vector();
    
    public HashSet<ModuleName> BlacklistedModules = [];

    public void DrawConfigUi() {
        var sampleNode = System.PartyListController.SampleNode;
        if (sampleNode is null) return;
        
        var configChanged = false;
        
        ImGuiTweaks.Header(Strings.DisplayOptions);
        using (var _ = ImRaii.PushIndent()) {
            configChanged |= ImGui.Checkbox(Strings.Enable, ref Enabled);
            configChanged |= ImGuiTweaks.Checkbox(Strings.SoloMode, ref SoloMode, Strings.SoloModeHelp);
            configChanged |= ImGui.Checkbox(Strings.SampleMode, ref SampleMode);
        }
        
        ImGuiTweaks.Header(Strings.DisplayStyle);
        using (var _ = ImRaii.PushIndent()) {
            if (ImGui.Checkbox("Play Animations", ref Animation)) {
                System.PartyListController.PlayAnimation(Animation ? 1 : 2);
                configChanged = true;
            }

            ImGuiHelpers.ScaledDummy(5.0f);
            
            if (ImGuiTweaks.ColorEditWithDefault(Strings.OutlineColor, ref OutlineColor, KnownColor.Red.Vector())) {
                System.PartyListController.UpdateOutlineColors();
                configChanged = true;
            }
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
            System.PartyListController.Save();
        }
    }
    
    public static PartyListConfig Load() 
        => Service.PluginInterface.LoadCharacterFile(Service.ClientState.LocalContentId, "PartyListOverlay.config.json", () => new PartyListConfig());

    public void Save()
        => Service.PluginInterface.SaveCharacterFile(Service.ClientState.LocalContentId, "PartyListOverlay.config.json", this);
}