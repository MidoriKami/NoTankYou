using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using Dalamud.Game.Addon.Lifecycle;
using Dalamud.Game.Addon.Lifecycle.AddonArgTypes;
using Dalamud.Interface;
using Dalamud.Interface.Utility;
using Dalamud.Interface.Utility.Raii;
using FFXIVClientStructs.FFXIV.Client.UI;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using FFXIVClientStructs.Interop;
using ImGuiNET;
using KamiLib.Components;
using KamiLib.Configuration;
using KamiLib.Extensions;
using NoTankYou.Classes;
using NoTankYou.Localization;

namespace NoTankYou.Controllers;

public unsafe class PartyListController : IDisposable {
    public PartyListConfig Config { get; private set; } = new();

    private PartyListMemberOverlay[] partyMembers = new PartyListMemberOverlay[8];

    private static WarningState SampleWarning => new() {
        Message = "NoTankYou Sample Warning",
        Priority = 100,
        IconId = 786,
        IconLabel = "Sample Action",
        SourceEntityId = Service.ClientState.LocalPlayer?.EntityId ?? 0xE000000,
        SourcePlayerName = "Sample Player",
        SourceModule = ModuleName.Test,
    };
    
    public PartyListController() {
        var partyListAddon = (AddonPartyList*) Service.GameGui.GetAddonByName("_PartyList");
        if (partyListAddon is not null) {
            AttachToNative(partyListAddon);
        }
        
        Service.AddonLifecycle.RegisterListener(AddonEvent.PostSetup, "_PartyList", OnPartyListSetup);
        Service.AddonLifecycle.RegisterListener(AddonEvent.PreFinalize, "_PartyList", OnPartyListFinalize);
    }

    public void Dispose() {
        Unload();
        
        Service.AddonLifecycle.UnregisterListener(OnPartyListSetup);
        Service.AddonLifecycle.UnregisterListener(OnPartyListFinalize);
    }
    
    public void Load() {
        Config = PartyListConfig.Load();
    }

    public void Unload() {
        foreach (var member in partyMembers) {
            member.Reset();
            member.Dispose();
        }
    }


    public void Update() {
        foreach (var member in partyMembers) {
            member.Update();
        }
    }

    public void Draw(List<WarningState> warnings) {
        if (!Config.Enabled) return;

        foreach (var partyMember in partyMembers) {
            partyMember.Reset();
        }
        
        if (Config.SampleMode) {
            partyMembers[0].DrawWarning(SampleWarning);
            return;
        }
        
        if (Config.SoloMode) {
            var warning = warnings
                .Where(warning => !Config.BlacklistedModules.Contains(warning.SourceModule))
                .Where(warning => warning.SourceEntityId == Service.ClientState.LocalPlayer?.EntityId)
                .MaxBy(warning => warning.Priority);
            
            partyMembers[0].DrawWarning(warning);
        }
        else {
            foreach (var index in Enumerable.Range(0, partyMembers.Length)) {
                var partyMember = partyMembers[index];
                var hudPartyMember = AgentHUD.Instance()->PartyMembers[index];
                
                var warning = warnings
                    .Where(warning => !Config.BlacklistedModules.Contains(warning.SourceModule))
                    .Where(warning => warning.SourceEntityId == hudPartyMember.EntityId)
                    .MaxBy(warning => warning.Priority);
                
                partyMember.DrawWarning(warning);
            }
        }
    }
    
    private void OnPartyListFinalize(AddonEvent type, AddonArgs args) {
        foreach (var node in partyMembers) {
            node.Dispose();
        }
    }

    private void OnPartyListSetup(AddonEvent type, AddonArgs args) {
        AttachToNative((AddonPartyList*)args.Addon);
    }

    public void DrawConfigUi()
        => Config.DrawConfigUi();

    private void AttachToNative(AddonPartyList* addonPartyList) {
        Service.Framework.RunOnFrameworkThread(() => {
            partyMembers = new PartyListMemberOverlay[8];

            foreach (var index in Enumerable.Range(0, 8)) {
                var partyMemberData = addonPartyList->PartyMembers.GetPointer(index);

                var partyMemberOverlay = new PartyListMemberOverlay(partyMemberData);
                
                partyMemberOverlay.EnableTooltip(addonPartyList);
                
                partyMembers[index] = partyMemberOverlay;
            }
        
            addonPartyList->UpdateCollisionNodeList(false);
            addonPartyList->UldManager.UpdateDrawNodeList();
        }); 
    }
}

public class PartyListConfig {
    public bool Enabled = true;
    public bool SoloMode;
    public bool SampleMode;
    
    public bool PlayerName = true;
    public bool JobIcon = true;
    public bool Animation = true;
    public float AnimationPeriod = 1000;
    
    public Vector4 OutlineColor  = KnownColor.Red.Vector();
    
    public HashSet<ModuleName> BlacklistedModules = [];

    public void DrawConfigUi() {
        var configChanged = false;
        
        ImGui.Text(Strings.DisplayOptions);
        ImGui.Separator();
        ImGuiHelpers.ScaledDummy(5.0f);
        
        using (var _ = ImRaii.PushIndent()) {
            configChanged |= ImGui.Checkbox(Strings.Enable, ref Enabled);
            configChanged |= ImGuiTweaks.Checkbox(Strings.SoloMode, ref SoloMode, Strings.SoloModeHelp);
            configChanged |= ImGui.Checkbox(Strings.SampleMode, ref SampleMode);
        }
        
        ImGuiHelpers.ScaledDummy(10.0f);
        ImGui.Text(Strings.DisplayStyle);
        ImGui.Separator();
        ImGuiHelpers.ScaledDummy(5.0f);
        
        using (var _ = ImRaii.PushIndent()) {
            configChanged |= ImGui.Checkbox(Strings.PlayerNames, ref PlayerName);
            configChanged |= ImGui.Checkbox(Strings.JobIcon, ref JobIcon);
            configChanged |= ImGui.Checkbox(Strings.Animation, ref Animation);
            
            ImGuiHelpers.ScaledDummy(5.0f);
            ImGui.SetNextItemWidth(ImGui.GetContentRegionAvail().X / 3.0f);
            configChanged |= ImGui.DragFloat(Strings.AnimationPeriod, ref AnimationPeriod, 5.0f, 1.0f, 30000.0f);
        }
        
        ImGuiHelpers.ScaledDummy(10.0f);
        ImGui.Text(Strings.DisplayColors);
        ImGui.Separator();
        ImGuiHelpers.ScaledDummy(5.0f);
        
        using (var _ = ImRaii.PushIndent()) {
            configChanged |= ImGuiTweaks.ColorEditWithDefault(Strings.OutlineColor, ref OutlineColor, KnownColor.Red.Vector());
        }
        
        ImGuiHelpers.ScaledDummy(10.0f);
        ImGui.Text(Strings.ModuleBlacklist);
        ImGui.Separator();
        ImGuiHelpers.ScaledDummy(5.0f);
        
        using (var _ = ImRaii.PushIndent()) {
            ImGui.Columns(2);

            foreach (var module in Enum.GetValues<ModuleName>()[..^1]) // Trim "Test" Module
            {
                var inHashset = BlacklistedModules.Contains(module);
                if(ImGui.Checkbox(module.GetDescription(), ref inHashset))
                {
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
        }
    }
    
    public static PartyListConfig Load() 
        => Service.PluginInterface.LoadCharacterFile(Service.ClientState.LocalContentId, "PartyListOverlay.config.json", () => new PartyListConfig());

    public void Save()
        => Service.PluginInterface.SaveCharacterFile(Service.ClientState.LocalContentId, "PartyListOverlay.config.json", this);
}