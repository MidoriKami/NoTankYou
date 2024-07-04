using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Numerics;
using Dalamud.Game.Addon.Lifecycle;
using Dalamud.Game.Addon.Lifecycle.AddonArgTypes;
using Dalamud.Interface;
using Dalamud.Interface.Utility;
using Dalamud.Interface.Utility.Raii;
using Dalamud.Utility;
using Dalamud.Utility.Numerics;
using FFXIVClientStructs.FFXIV.Client.Graphics;
using FFXIVClientStructs.FFXIV.Client.UI;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using FFXIVClientStructs.FFXIV.Component.GUI;
using FFXIVClientStructs.Interop;
using ImGuiNET;
using KamiLib.Classes;
using KamiLib.Configuration;
using KamiLib.Extensions;
using KamiToolKit.Classes;
using KamiToolKit.Nodes;
using NoTankYou.Classes;
using NoTankYou.Localization;

namespace NoTankYou.Controllers;

public unsafe class PartyListController : IDisposable {
    private static readonly Stopwatch AnimationStopwatch = Stopwatch.StartNew();
    private static bool AnimationState => !System.PartyListController.Config.Animation || AnimationStopwatch.ElapsedMilliseconds < System.PartyListController.Config.AnimationPeriod / 2.0f;

    private PartyListConfig Config { get; set; } = new();

    private readonly IconImageNode[] jobIconWarningNodes = new IconImageNode[8];
    private readonly ImageNode[] warningTypeNodes = new ImageNode[8];
    private ByteColor originalOutlineColor;
    private bool isAttached;

    private static AddonPartyList* AddonPartyList => (AddonPartyList*) Service.GameGui.GetAddonByName("_PartyList");

    public void Dispose() {
        Unload();
    }
    
    public void Load() {
        Config = PartyListConfig.Load();
        
        Service.AddonLifecycle.RegisterListener(AddonEvent.PostSetup, "_PartyList", OnPartyListSetup);
        Service.AddonLifecycle.RegisterListener(AddonEvent.PreFinalize, "_PartyList", OnPartyListFinalize);
        
        if (AddonPartyList is not null) {
            AttachToNative(AddonPartyList);
        }
    }

    public void Unload() {
        Service.AddonLifecycle.UnregisterListener(OnPartyListSetup);
        Service.AddonLifecycle.UnregisterListener(OnPartyListFinalize);
        
        if (AddonPartyList is not null) {
            DetachFromNative(AddonPartyList);
        }
    }

    public void DrawConfigUi()
        => Config.DrawConfigUi();
    
    public void Update() {
        if (AnimationStopwatch.ElapsedMilliseconds > System.PartyListController.Config.AnimationPeriod) {
            AnimationStopwatch.Restart();
        }
    }

    public void Draw(List<WarningState> warnings) {
        if (!Config.Enabled) return;
        if (!isAttached) return;

        ResetPartyMembers();
        
        if (Config.SampleMode) {
            DrawWarning(0, ModuleController.SampleWarning);
            return;
        }
        
        if (Config.SoloMode) {
            var warning = warnings
                .Where(warning => !Config.BlacklistedModules.Contains(warning.SourceModule))
                .Where(warning => warning.SourceEntityId == Service.ClientState.LocalPlayer?.EntityId)
                .MaxBy(warning => warning.Priority);
            
            DrawWarning(0, warning);
        }
        else {
            foreach(var index in Enumerable.Range(0, 8)) {
                var hudPartyMember = AgentHUD.Instance()->PartyMembers.GetPointer(index);
                
                var warning = warnings
                    .Where(warning => !Config.BlacklistedModules.Contains(warning.SourceModule))
                    .Where(warning => warning.SourceEntityId == hudPartyMember->EntityId)
                    .MaxBy(warning => warning.Priority);
                
                DrawWarning(index, warning);
            }
        }
    }

    private void OnPartyListSetup(AddonEvent type, AddonArgs args) {
        AttachToNative((AddonPartyList*)args.Addon);
    }
    
    private void AttachToNative(AddonPartyList* addonPartyList) {
        Service.Framework.RunOnFrameworkThread(() => {
            if (isAttached) {
                Service.Log.Info("Attempted to Attach to PartyList after already attached.");
                return;
            }
            
            originalOutlineColor = addonPartyList->Pet.Name->EdgeColor;
            
            foreach (var index in Enumerable.Range(0, 8)) {
                ref var partyMember = ref AddonPartyList->PartyMembers[index];

                warningTypeNodes[index] = new ImageNode {
                    NodeID = 20000 + partyMember.PartyMemberComponent->OwnerNode->NodeId,
                    NodeFlags = NodeFlags.Visible,
                    Size = new Vector2(24.0f, 24.0f),
                    X = partyMember.ClassJobIcon->GetXFloat() + 16.0f,
                    Y = partyMember.ClassJobIcon->GetYFloat(),
                    IsVisible = false,
                };
                
                System.NativeController.AttachToComponent(
                    warningTypeNodes[index],
                    (AtkUnitBase*)addonPartyList, 
                    partyMember.PartyMemberComponent, 
                    (AtkResNode*)partyMember.ClassJobIcon, 
                    NodePosition.AfterTarget);
                
                jobIconWarningNodes[index] = new IconImageNode {
		            NodeID = 10000 + partyMember.PartyMemberComponent->OwnerNode->NodeId,
		            NodeFlags = NodeFlags.Visible,
		            Size = new Vector2(32.0f, 32.0f),
		            X = partyMember.ClassJobIcon->GetXFloat(),
		            Y = partyMember.ClassJobIcon->GetYFloat(),
		            IsVisible = false,
                    IconId = 60074,
                };
                
                System.NativeController.AttachToComponent(
                    jobIconWarningNodes[index],
                    (AtkUnitBase*)addonPartyList, 
                    partyMember.PartyMemberComponent, 
                    (AtkResNode*)partyMember.ClassJobIcon, 
                    NodePosition.AfterTarget);
            }

            isAttached = true;
        }); 
    }
    
    private void OnPartyListFinalize(AddonEvent type, AddonArgs args) {
        DetachFromNative((AddonPartyList*)args.Addon);
    }
    
    private void DetachFromNative(AddonPartyList* addonPartyList) {
        Service.Framework.RunOnFrameworkThread(() => {
            if (!isAttached) {
                Service.Log.Info("Attempted to Detach from PartyList without being attached.");
                return;
            }

            foreach (var index in Enumerable.Range(0, 8)) {
                ref var partyMember = ref AddonPartyList->PartyMembers[index];
                                
                System.NativeController.DetachFromComponent(warningTypeNodes[index], (AtkUnitBase*)addonPartyList, partyMember.PartyMemberComponent);
                warningTypeNodes[index].Dispose();
                
                System.NativeController.DetachFromComponent(jobIconWarningNodes[index], (AtkUnitBase*)addonPartyList, partyMember.PartyMemberComponent);
                jobIconWarningNodes[index].Dispose();
            }
            
            ResetPartyMembers();

            isAttached = false;
        });
    }
    
    private void ResetPartyMembers() {
	    if (AddonPartyList is null) return;
	    
        foreach (var index in Enumerable.Range(0, 8)) {
            ref var memberComponent = ref AddonPartyList->PartyMembers[index];

            memberComponent.Name->EdgeColor = originalOutlineColor;
            memberComponent.ClassJobIcon->ToggleVisibility(true);
        }
    }

    private void DrawWarning(int index, WarningState? warning) {
	    if (AddonPartyList is null) return;
        if (warning is null) return;
		
		if (System.PartyListController.Config.JobIcon) {
			AnimateWarningIcon(index, warning);
		}

		if (System.PartyListController.Config.PlayerName) {
			AnimePlayerNameColor(index);
		}
    }
    
    private void AnimateWarningIcon(int index, WarningState warning) {
	    if (AddonPartyList is null) return;
        ref var memberComponent = ref AddonPartyList->PartyMembers[index];

	    if (AnimationState) {
            jobIconWarningNodes[index].IsVisible = true;
            memberComponent.ClassJobIcon->ToggleVisibility(false);
            warningTypeNodes[index].IsVisible = true;
            SetWarningTypeTexture(warningTypeNodes[index], warning);
	    }
	    else {
            jobIconWarningNodes[index].IsVisible = false;
            memberComponent.ClassJobIcon->ToggleVisibility(true);
            warningTypeNodes[index].IsVisible = false;
        }
    }

    private void AnimePlayerNameColor(int index) {
	    if (AddonPartyList is null) return;
        ref var memberComponent = ref AddonPartyList->PartyMembers[index];

	    if (AnimationState) {
            memberComponent.Name->EdgeColor = Config.OutlineColor.ToByteColor();
	    }
	    else {
            memberComponent.Name->EdgeColor = originalOutlineColor;
	    }
	}

    private void SetWarningTypeTexture(ImageNode imageNode, WarningState warningState) {
        if (warningState.SourceModule is ModuleName.Tanks) {
            if (imageNode.LoadedIconId is not uint.MaxValue) {
                imageNode.LoadTexture("ui/uld/fourth/LFG_hr1.tex");
                imageNode.TextureCoordinates = new Vector2(0.0f, 216.0f);
                imageNode.ImageNodeFlags = 0;
                imageNode.Size = new Vector2(56.0f, 56.0f);
                imageNode.Scale = new Vector2(24.0f / 56.0f);  
            }
        }
        else {
            var warningIcon = warningState.SourceModule.GetAttribute<ModuleIconAttribute>()!.SimpleIcon;
            if (imageNode.LoadedIconId != warningIcon) {
                imageNode.LoadIcon(warningIcon);
            }
        }
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
        }
    }
    
    public static PartyListConfig Load() 
        => Service.PluginInterface.LoadCharacterFile(Service.ClientState.LocalContentId, "PartyListOverlay.config.json", () => new PartyListConfig());

    public void Save()
        => Service.PluginInterface.SaveCharacterFile(Service.ClientState.LocalContentId, "PartyListOverlay.config.json", this);
}