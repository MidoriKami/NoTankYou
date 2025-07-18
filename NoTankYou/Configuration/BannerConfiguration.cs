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

public class BannerConfig {
    public bool Enabled = true;
    public bool SoloMode;
    public bool SampleMode; 
    public bool EnableAnimation = true;

    public BannerOverlayDisplayMode DisplayMode = BannerOverlayDisplayMode.List;

    public HashSet<ModuleName> BlacklistedModules = [];

    public void DrawConfigUi() {
        var listNode = System.BannerController.BannerListNode;
        if (listNode is null) return;
        
        var sampleNode = System.BannerController.SampleNode;
        
        var configChanged = false;
        
        ImGuiTweaks.Header(Strings.DisplayOptions);
        using (var _ = ImRaii.PushIndent()) {
            configChanged |= ImGui.Checkbox(Strings.Enable, ref Enabled);
            configChanged |= ImGuiTweaks.Checkbox(Strings.SoloMode, ref SoloMode, Strings.SoloModeHelp);
            configChanged |= ImGui.Checkbox(Strings.SampleMode, ref SampleMode);
        }
        
        ImGuiTweaks.Header(Strings.Positioning);
        using (var _ = ImRaii.PushIndent()) {
            var nodePosition = listNode.Position;
            if (ImGui.DragFloat2(Strings.Position, ref nodePosition)) {
                listNode.Position = nodePosition;
                configChanged = true;
            }
            
            var nodesize = listNode.Size;
            if (ImGui.DragFloat2("Size", ref nodesize)) {
                listNode.Size = nodesize;
                configChanged = true;
            }
            
            ImGuiHelpers.ScaledDummy(5.0f);
            
            var nodeScale = listNode.Scale.X;
            if (ImGui.DragFloat("Scale", ref nodeScale, 0.001f, 0.05f, 5.0f)) {
                listNode.Scale = new Vector2(nodeScale,  nodeScale);
                configChanged = true;
            }
            
            ImGuiHelpers.ScaledDummy(5.0f);

            var repositionMode = listNode.EnableMoving;
            if (ImGui.Checkbox("Enable Repositioning", ref repositionMode)) {
                listNode.EnableMoving = repositionMode;
                SampleMode = true;
            }
            
            var resizeMode = listNode.EnableResizing;
            if (ImGui.Checkbox("Enable Resizing", ref resizeMode)) {
                listNode.EnableResizing = resizeMode;
                SampleMode = true;
            }
        }
        
        ImGuiTweaks.Header(Strings.DisplayMode);
        using (var _ = ImRaii.PushIndent()) {
            configChanged |= ImGuiTweaks.EnumCombo(Strings.DisplayMode, ref DisplayMode);
            
            var layoutDirection = listNode.LayoutOrientation;
            if (ImGuiTweaks.EnumCombo("Layout Orientation", ref layoutDirection)) {
                listNode.LayoutOrientation = layoutDirection;
                configChanged = true;
            }
            
            var layoutAnchor = listNode.LayoutAnchor;
            if (ImGuiTweaks.EnumCombo("Layout Anchor", ref layoutAnchor)) {
                listNode.LayoutAnchor = layoutAnchor;
                configChanged = true;
            }
            
            ImGuiHelpers.ScaledDummy(5.0f);
            
            var showBackground = listNode.ShowBackground;
            if (ImGui.Checkbox("Show Background", ref showBackground)) {
                listNode.ShowBackground = showBackground;
                configChanged = true;
            }
            
            var showBorder = listNode.ShowBorder;
            if (ImGui.Checkbox("Show Border", ref showBorder)) {
                listNode.ShowBorder = showBorder;
                configChanged = true;
            }
            
            var clipContents = listNode.ClipListContents;
            if (ImGui.Checkbox("Clip Contents", ref clipContents)) {
                listNode.ClipListContents = clipContents;
                configChanged = true;
            }
        }
        
        ImGuiTweaks.Header(Strings.DisplayStyle);
        using (var _ = ImRaii.PushIndent()) {
            var showWarningImage = sampleNode.ShowWarningImage;
            if (ImGui.Checkbox(Strings.WarningShield, ref showWarningImage)) {
                sampleNode.ShowWarningImage = showWarningImage;
                sampleNode.Save();
                configChanged = true;
            }
            
            var showMessageText = sampleNode.ShowMessageText;
            if (ImGui.Checkbox(Strings.WarningText, ref showMessageText)) {
                sampleNode.ShowMessageText = showMessageText;
                sampleNode.Save();
                configChanged = true;
            }
            
            var showPlayerText = sampleNode.ShowPlayerText;
            if (ImGui.Checkbox(Strings.PlayerNames, ref showPlayerText)) {
                sampleNode.ShowPlayerText = showPlayerText;
                sampleNode.Save();
                configChanged = true;
            }
            
            var showActionName = sampleNode.ShowActionName;
            if (ImGui.Checkbox(Strings.ActionName, ref showActionName)) {
                sampleNode.ShowActionName = showActionName;
                sampleNode.Save();
                configChanged = true;
            }
            
            var showActionIcon = sampleNode.ShowActionIcon;
            if (ImGui.Checkbox(Strings.ActionIcon, ref showActionIcon)) {
                sampleNode.ShowActionIcon = showActionIcon;
                sampleNode.Save();
                configChanged = true;
            }

            ImGuiHelpers.ScaledDummy(5.0f);
            
            if (ImGui.Checkbox("Play Animations", ref EnableAnimation)) {
                System.BannerController.PlayAnimation(EnableAnimation ? 1 : 2);
                configChanged = true;
            }
                        
            ImGuiHelpers.ScaledDummy(5.0f);

            var backgroundColor= listNode.BackgroundColor;
            if (ImGuiTweaks.ColorEditWithDefault("Background Color", ref backgroundColor, KnownColor.Aqua.Vector() with { W = 0.15f })) {
                listNode.BackgroundColor =  backgroundColor;
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
            System.BannerController.Save();
        }
    }
    
    public static BannerConfig Load() 
        => Service.PluginInterface.LoadCharacterFile(Service.ClientState.LocalContentId, "BannerDisplay.config.json", () => new BannerConfig());

    private void Save()
        => Service.PluginInterface.SaveCharacterFile(Service.ClientState.LocalContentId, "BannerDisplay.config.json", this);
}