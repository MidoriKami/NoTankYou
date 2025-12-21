using System;
using System.Collections.Generic;
using System.Linq;
using FFXIVClientStructs.FFXIV.Client.UI;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using FFXIVClientStructs.Interop;
using KamiToolKit.Classes.Controllers;
using NoTankYou.Classes;
using NoTankYou.Configuration;
using NoTankYou.Extensions;

namespace NoTankYou.Controllers;

public unsafe class PartyListController : IDisposable {
    public PartyListConfig Config { get; private set; } = new();

    private readonly List<PartyListOverlay> partyMemberNodes = [];

    private static AddonPartyList* PartyList => Services.GameGui.GetAddonByName<AddonPartyList>("_PartyList");
    
    private readonly AddonController<AddonPartyList> partyListController;

    private int lastMemberCount;
    
    public PartyListController() {
        partyListController = new AddonController<AddonPartyList>("_PartyList");
        
        partyListController.OnPreEnable += LoadConfig;
        partyListController.OnPostDisable += UnloadNodes;
        partyListController.OnUpdate += OnUpdate;
    }

    public void Dispose()
        => partyListController.Dispose();

    public void Enable()
        => partyListController.Enable();

    public void Disable()
        => partyListController.Disable();

    private void UnloadNodes(AddonPartyList* addon)
        => RemoveAllNodes();

    private void LoadConfig(AddonPartyList* addon)
        => Config = PartyListConfig.Load();

    public void DrawConfigUi()
        => Config.DrawConfigUi();

    private void OnUpdate(AddonPartyList* addon) {
        if (lastMemberCount != addon->MemberCount) {
            RemoveAllNodes();
        }
        
        lastMemberCount = addon->MemberCount;
    }

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
                .Where(warning => warning.SourceEntityId == Services.ObjectTable.LocalPlayer?.EntityId)
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

        var memberStruct = GetPartyMemberStruct(warning);
        if (memberStruct is null) return;
        
        var newPartyMemberNode = new PartyListOverlay {
            Warning = warning,
            MemberStruct = memberStruct,
        };
        
        newPartyMemberNode.Attach();
        partyMemberNodes.Add(newPartyMemberNode);
    }

    private void RemoveNode(PartyListOverlay node) {
        node.Detach();
        partyMemberNodes.Remove(node);
    }

    private int? GetHudMemberIndex(WarningState warningState)
        => GetHudMember(warningState)?.Index;
    
    private HudPartyMember? GetHudMember(WarningState warningState)
        => AgentHUD.Instance()->GetMember(warningState.SourceEntityId);

    private AddonPartyList.PartyListMemberStruct* GetPartyMemberStruct(WarningState warningState) {
        if (PartyList is null) return null;
        if (GetHudMemberIndex(warningState) is not {} memberIndex) return null;

        return PartyList->PartyMembers.GetPointer(memberIndex);
    }

    public void OnConfigChanged() {
        foreach (var node in partyMemberNodes) {
            node.ApplyConfigurationOptions();
        }
    }
}
