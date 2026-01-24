using System;
using System.Collections.Generic;
using System.Linq;
using FFXIVClientStructs.FFXIV.Client.UI;
using KamiToolKit;
using KamiToolKit.Classes;
using KamiToolKit.Controllers;
using KamiToolKit.Extensions;
using NoTankYou.Classes;
using NoTankYou.Enums;
using NoTankYou.Extensions;

namespace NoTankYou.Features.PartyList;

public unsafe class PartyList : FeatureBase {
    public override ModuleInfo ModuleInfo => new() {
        DisplayName = "Party List",
        FileName = "PartyListOverlay",
        IconId = 0,
        Type = ModuleType.GeneralFeatures,
    };

    private AddonController<AddonPartyList>? partyListController;
    private readonly List<PartyListMemberNode> partyListNodes = [];

    public override NodeBase DisplayNode => new PartyListConfigNode(this);
    
    public PartyListConfig Config = null!;
    
    public static PartyListConfig? PartyListConfig { get; private set; }

    protected override void OnFeatureLoad() {
        Config = Utilities.Config.LoadCharacterConfig<PartyListConfig>($"{ModuleInfo.FileName}.config.json");
        if (Config is null) throw new Exception("Failed to load config file");
        
        Config.FileName = ModuleInfo.FileName;
        PartyListConfig = Config;
    }

    protected override void OnFeatureUnload() {
        Config = null!;
        PartyListConfig = null;
    }

    protected override void OnFeatureEnable() {
        partyListController = new AddonController<AddonPartyList>("_PartyList");

        partyListController.OnAttach += AttachNodes;
        partyListController.OnUpdate += OnPartyListUpdate;
        partyListController.OnDetach += DetachNodes;
        
        partyListController.Enable();
    }

    protected override void OnFeatureDisable() {
        partyListController?.Dispose();
        partyListController = null;
    }
        
    protected override void OnFeatureUpdate() {
        if (Config.SavePending) {
            Services.PluginLog.Debug($"Saving {ModuleInfo.DisplayName} config");
            Config.Save();
        }
    }

    private void AttachNodes(AddonPartyList* addon) {
        foreach (uint nodeId in Enumerable.Range(10, 8)) {
            var partyMemberNode = addon->GetComponentNodeById(nodeId);
            if (partyMemberNode is not null) {
                var backgroundNode = new PartyListBackgroundNode {
                    NodeId = partyMemberNode->NodeId * 100000,
                    Size = partyMemberNode->AtkResNode.Size,
                    Position = partyMemberNode->AtkResNode.Position,
                    IsVisible = false,
                };
                backgroundNode.AttachNode(partyMemberNode, NodePosition.BeforeTarget);

                var foregroundNode = new PartyListForegroundNode {
                    NodeId = partyMemberNode->NodeId * 100000 + 1,
                    Size = partyMemberNode->AtkResNode.Size,
                    Position = partyMemberNode->AtkResNode.Position,
                    IsVisible = false,
                };
                foregroundNode.AttachNode(partyMemberNode, NodePosition.AfterTarget);
                
                partyListNodes.Add(new PartyListMemberNode {
                    Background = backgroundNode,
                    Foreground = foregroundNode,
                });
            }
        }
    }

    private void DetachNodes(AddonPartyList* addon) {
        foreach (var node in partyListNodes) {
            node.Background.Dispose();
            node.Foreground.Dispose();
        }

        partyListNodes.Clear();
    }
    
    private void OnPartyListUpdate(AddonPartyList* addon) {
        if (!IsEnabled) return;
        
        var filteredWarning = System.WarningController.ActiveWarnings
            .Where(warning => !Config.BlacklistedModules.Contains(warning.SourceModule))
            .Where(warning => !Config.SoloMode || warning.SourceCharacter->ObjectIndex is 0)
            .Where(warning => warning.SourceCharacter->HudIndex is not null);

        var groupedWarnings = filteredWarning
            .GroupBy(warning => warning.SourceCharacter->HudIndex!.Value)
            .ToDictionary(pairing => pairing.Key, pairing => pairing.ToList());

        foreach (var index in Enumerable.Range(0, partyListNodes.Count)) {
            if (groupedWarnings.TryGetValue(index, out var warnings)) {
                partyListNodes[index].ActiveWarning = warnings.MaxBy(warning => warning.Priority);
            }
            else {
                partyListNodes[index].ActiveWarning = null;
            }
            
            partyListNodes[index].Update();
        }
    }
}
