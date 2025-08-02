using System.Collections.Generic;
using System.Linq;
using FFXIVClientStructs.FFXIV.Client.UI;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using FFXIVClientStructs.Interop;
using KamiLib.Extensions;
using KamiToolKit;
using NoTankYou.Classes;
using NoTankYou.Configuration;
using NoTankYou.Extensions;

namespace NoTankYou.Controllers;

public unsafe class PartyListController : AddonController<AddonPartyList> {
    public PartyListConfig Config { get; set; } = new();

    private readonly List<PartyListOverlay> partyMemberNodes = [];

    private static AddonPartyList* PartyList => Service.GameGui.GetAddonByName<AddonPartyList>("_PartyList");
    
    public PartyListController() : base(Service.PluginInterface) {
        PreEnable += LoadConfig;
        PostDisable += UnloadNodes;
    }

    private void UnloadNodes(AddonPartyList* addon)
        => RemoveAllNodes();

    private void LoadConfig(AddonPartyList* addon)
        => Config = PartyListConfig.Load();

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

        var memberStruct = GetPartyMemberStruct(warning);
        if (memberStruct is null) return;
        
        var newPartyMemberNode = new PartyListOverlay {
            Warning = warning,
            MemberStruct = memberStruct,
            EnableAnimation = Config.Animation,
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
}
