using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using FFXIVClientStructs.FFXIV.Client.UI;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiLib.Extensions;
using KamiToolKit;
using KamiToolKit.Classes;
using NoTankYou.Classes;
using NoTankYou.Configuration;
using NoTankYou.Extensions;

namespace NoTankYou.Controllers;

public unsafe class PartyListController : AddonController<AddonPartyList> {
    public PartyListConfig Config { get; set; } = new();

    private readonly List<PartyListOverlayNode> partyMemberNodes = [];

    private static AddonPartyList* PartyList => Service.GameGui.GetAddonByName<AddonPartyList>("_PartyList");

    public PartyListOverlayNode? SampleNode;

    public PartyListController() : base(Service.PluginInterface) {
        PreEnable += LoadConfig;
        PostDisable += UnloadNodes;
    }

    private void UnloadNodes(AddonPartyList* addon)
        => RemoveAllNodes();

    private void LoadConfig(AddonPartyList* addon) {
        Config = PartyListConfig.Load();

        SampleNode = new PartyListOverlayNode {
            Size = new Vector2(448.0f, 64.0f), 
            IsVisible = true,
        };
    }

    public void DrawConfigUi()
        => Config.DrawConfigUi();
    
    public void UpdateWarnings(IEnumerable<WarningState> warningStates) {
        if (!Config.Enabled) {
            RemoveAllNodes();
            return;
        }

        var filteredWarningStates = warningStates
            .Where(warning => !Config.BlacklistedModules.Contains(warning.SourceModule)).ToList();
        
        if (Config.SampleMode) {
            filteredWarningStates.Add(ModuleController.SampleWarning);
        }
        
        filteredWarningStates = filteredWarningStates.GroupBy(warnings => warnings.SourceEntityId)
            .Select(warningGroups => warningGroups.OrderByDescending(warning => warning.Priority).First())
            .ToList();
        
        if (Config.SoloMode) {
            filteredWarningStates = filteredWarningStates
                .Where(warning => warning.SourceEntityId == Service.ClientState.LocalPlayer?.EntityId)
                .ToList();
        }
        
        SyncWarnings(filteredWarningStates);
    }

    private void RemoveAllNodes() {
        foreach (var node in partyMemberNodes.ToList()) {
            RemoveNode(node);
        }
    }

    private void SyncWarnings(List<WarningState> warningStates) {
        if (warningStates.Count is 0) {
            RemoveAllNodes();
        }
        else {
            var toRemove =  partyMemberNodes.Where(node => !warningStates.Any(warning => node.Warning == warning)).ToList();
            foreach (var node in toRemove) {
                RemoveNode(node);
            }
        
            var toAdd = warningStates.Where(warning => !partyMemberNodes.Any(node => node.Warning == warning)).ToList();
            foreach (var warning in toAdd) {
                AddNode(warning);
            }
        }
    }

    private void AddNode(WarningState warning) {
        if (PartyList is null) return;

        if (GetPartyMemberStruct(warning) is not {} memberStruct) return;
        
        var memberNode = GetPartyMemberNode(warning);
        if (memberNode is null) return;
        
        var newPartyMemberNode = new PartyListOverlayNode {
            Size = new Vector2(memberNode->Width, memberNode->Height),
            IsVisible = true,
            NameText = memberStruct.Name->GetText().ToString(),
            Priority = 1,
        };
        
        newPartyMemberNode.Load();
        newPartyMemberNode.Warning = warning;

        System.NativeController.AttachNode(newPartyMemberNode, memberNode, NodePosition.BeforeTarget);
        
        memberStruct.ClassJobIcon->ToggleVisibility(false);
        memberStruct.Name->FontSize = 0;

        partyMemberNodes.Add(newPartyMemberNode);
    }

    private void RemoveNode(PartyListOverlayNode node) {
        if (node.Warning is null) return;
        if (GetPartyMemberStruct(node.Warning) is not {} memberStruct) return;
        
        memberStruct.ClassJobIcon->ToggleVisibility(true);
        memberStruct.Name->FontSize = 14;
        
        System.NativeController.DetachNode(node, node.Dispose);
        partyMemberNodes.Remove(node);
    }

    private int? GetHudMemberIndex(WarningState warningState)
        => GetHudMember(warningState)?.Index;
    
    private HudPartyMember? GetHudMember(WarningState warningState)
        => AgentHUD.Instance()->GetMember(warningState.SourceEntityId);

    private AddonPartyList.PartyListMemberStruct? GetPartyMemberStruct(WarningState warningState) {
        if (PartyList is null) return null;
        if (GetHudMemberIndex(warningState) is not {} memberIndex) return null;

        return PartyList->PartyMembers[memberIndex];
    }

    private AtkComponentNode* GetPartyMemberNode(WarningState warningState) {
        if (PartyList is null) return null;
        if (GetPartyMemberStruct(warningState) is not {} memberStruct) return null;

        return memberStruct.PartyMemberComponent->OwnerNode;
    }
    
    public void Save() {
        SampleNode?.Save();
        RemoveAllNodes();
    }

    public void UpdateOutlineColors() {
        foreach (var node in partyMemberNodes) {
            node.UpdateNameColor();
        }
    }
}
