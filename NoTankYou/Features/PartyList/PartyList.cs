using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FFXIVClientStructs.FFXIV.Client.UI;
using KamiToolKit;
using KamiToolKit.Classes;
using KamiToolKit.Controllers;
using KamiToolKit.Extensions;
using NoTankYou.Classes;
using NoTankYou.Enums;

namespace NoTankYou.Features.PartyList;

public class PartyList : FeatureBase {
    public override ModuleInfo ModuleInfo => new() {
        DisplayName = "Party List",
        FileName = "PartyListOverlay",
        IconId = 0,
        Type = ModuleType.WarningDisplays,
    };

    private AddonController<AddonPartyList>? partyListController;
    private readonly List<PartyListMemberNode> partyListNodes = [];

    public override NodeBase DisplayNode => new PartyListConfigNode(this);

    public PartyListConfig Config = null!;

    public static PartyListConfig? PartyListConfig { get; private set; }

    protected override async Task OnFeatureLoad() {
        Config = await Utilities.Config.LoadCharacterConfig<PartyListConfig>($"{ModuleInfo.FileName}.config.json");
        if (Config is null) throw new Exception("Failed to load config file");

        Config.FileName = ModuleInfo.FileName;
        PartyListConfig = Config;
    }

    protected override Task OnFeatureUnload() {
        Config = null!;
        PartyListConfig = null;

        return Task.CompletedTask;
    }

    protected override async Task OnFeatureEnable() {
        unsafe {
            partyListController = new AddonController<AddonPartyList> {
                AddonName = "_PartyList",
                OnSetup = AttachNodes,
                OnUpdate = OnPartyListUpdate,
                OnFinalize = DetachNodes,
            };
        }

        await Services.Framework.Run(partyListController.Enable);
    }

    protected override async Task OnFeatureDisable() {
        await Services.Framework.Run(() => partyListController?.Dispose());
        partyListController = null;
    }

    protected override void OnFeatureUpdate() {
        if (Config.SavePending) {
            Services.PluginLog.Debug($"Saving {ModuleInfo.DisplayName} config");
            Task.Run(Config.Save);
        }
    }

    private unsafe void AttachNodes(AddonPartyList* addon) {
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

    private unsafe void DetachNodes(AddonPartyList* addon) {
        foreach (var node in partyListNodes) {
            node.Background.Dispose();
            node.Foreground.Dispose();
        }

        partyListNodes.Clear();
    }

    private unsafe void OnPartyListUpdate(AddonPartyList* addon) {
        if (!IsEnabled) return;

        var filteredWarning = System.WarningController.ActiveWarnings
            .Where(warning => !Config.DisabledModules.Contains(warning.SourceModule))
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
