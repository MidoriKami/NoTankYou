using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using Dalamud.Interface;
using FFXIVClientStructs.FFXIV.Client.UI;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiLib.Configuration;
using KamiToolKit;
using KamiToolKit.Classes;
using KamiToolKit.Classes.TimelineBuilding;
using KamiToolKit.Nodes;
using NoTankYou.Classes;
using NoTankYou.Configuration;
using KamiToolKit.Extensions;
using KamiToolKit.System;

namespace NoTankYou.Controllers;

public unsafe class BannerController : NameplateAddonController {
    private BannerConfig Config { get; set; } = new();

    private SimpleOverlayNode? overlayRootNode;
    public ListBoxNode? BannerListNode;
    private readonly List<BannerOverlayNode> nodeList = [];

    public readonly BannerOverlayNode SampleNode = new() {
        Size = new Vector2(448.0f, 64.0f), 
        IsVisible = true,
    };

    private static string BannerListPath => Service.PluginInterface.GetCharacterFileInfo(Service.ClientState.LocalContentId, "BannerList.style.json").FullName;

    public BannerController() : base(Service.PluginInterface) {
        PreEnable += LoadConfig;
        OnAttach += AttachNodes;
        OnDetach += DetachNodes;
    }

    private void LoadConfig(AddonNamePlate* addon)
        => Config = BannerConfig.Load();

    private void AttachNodes(AddonNamePlate* addonNamePlate) {
        overlayRootNode = new SimpleOverlayNode {
            NodeId = 100000003,
            Size = addonNamePlate->AtkUnitBase.Size(),
            IsVisible = true,
        };
        System.NativeController.AttachNode(overlayRootNode, addonNamePlate->RootNode, NodePosition.AsFirstChild);

        BannerListNode = new ListBoxNode {
            Position = new Vector2(700.0f, 400.0f),
            Size = new Vector2(448.0f, 320.0f),
            LayoutAnchor = LayoutAnchor.TopLeft,
            IsVisible = true,
            LayoutOrientation = LayoutOrientation.Vertical,
            NodeId = 2,
            BackgroundColor = KnownColor.Aqua.Vector() with { W = 0.15f },
            ShowBackground = false,
            ItemMargin = new Spacing(5.0f),
            OnEditComplete = () => BannerListNode?.Save(BannerListPath),
            ClipListContents = true,
        };
        System.NativeController.AttachNode(BannerListNode, overlayRootNode);
        BannerListNode.Load(BannerListPath);

        BannerListNode.AddTimeline(new TimelineBuilder()
            .BeginFrameSet(1, 60)
            .AddLabel(1, 1, AtkTimelineJumpBehavior.Start, 0) // Label 1: Pulsing Animation
            .AddLabel(30, 0, AtkTimelineJumpBehavior.LoopForever, 1)
            .AddLabel(31, 2, AtkTimelineJumpBehavior.Start, 0) // Label 2: No Animation
            .AddLabel(60, 0, AtkTimelineJumpBehavior.LoopForever, 2)
            .EndFrameSet()
            .Build());
        
        BannerListNode.Timeline?.PlayAnimation(Config.EnableAnimation ? 1 : 2);
    }

    private void DetachNodes(AddonNamePlate* addonNamePlate) {
        System.NativeController.DetachNode(BannerListNode, () => {
            BannerListNode?.Dispose();
            BannerListNode = null;
        });
        
        System.NativeController.DetachNode(overlayRootNode, () => {
            overlayRootNode?.Dispose();
            overlayRootNode = null;
        });
    }

    public void DrawConfigUi()
        => Config.DrawConfigUi();

    public void UpdateWarnings(List<WarningState> warningStates) {
        if (BannerListNode is null) return;
        if (!Config.Enabled) {
            RemoveAllNodes();
            return;
        }
        
        var filteredWarningStates = warningStates
            .Where(warning => !Config.BlacklistedModules.Contains(warning.SourceModule))
            .OrderByDescending(warning => warning.Priority)
            .ToList();

        if (Config.SoloMode) {
            filteredWarningStates = filteredWarningStates
                .Where(warning => warning.SourceEntityId == Service.ClientState.LocalPlayer?.EntityId)
                .ToList();
        }

        if (Config.SampleMode) {
            filteredWarningStates.Add(ModuleController.SampleWarning);
        }

        switch (Config.DisplayMode) {
            case BannerOverlayDisplayMode.TopPriority when filteredWarningStates.MaxBy(warning => warning.Priority) is { } topWarning:
                SyncWarnings([ topWarning ]);
                break;
            
            case BannerOverlayDisplayMode.List:
                SyncWarnings(filteredWarningStates);
                break;
        }
    }

    private void AddNode(WarningState warningState) {
        if (BannerListNode is null) return;
        
        var newBannerNode = new BannerOverlayNode {
            Size = new Vector2(448.0f, 64.0f),
            IsVisible = true,
        };
                
        newBannerNode.Load();
        newBannerNode.Warning = warningState;

        BannerListNode.AddNode(newBannerNode);
        nodeList.Add(newBannerNode);
    }

    private void RemoveAllNodes() {
        if (BannerListNode is null) return;
       
        foreach (var node in nodeList.ToList()) {
            RemoveNode(node);
        }
    }
    
    private void RemoveNode(BannerOverlayNode node) {
        if (BannerListNode is null) return;
        
        BannerListNode.RemoveNode(node);
        nodeList.Remove(node);
    }

    public void Save() {
        BannerListNode?.Save(BannerListPath);
        SampleNode.Save();

        RemoveAllNodes();
    }

    public void DisablePreviewMode() {
        Config.SampleMode = false;
        SampleNode.Save();
    }

    private void SyncWarnings(List<WarningState> warningStates) {
        if (BannerListNode is null) return;
        
        if (warningStates.Count is 0) {
            RemoveAllNodes();
        }
        else {
            // Get a list of nodes that need to be removed
            var toRemove = nodeList.Where(node => !warningStates.Any(warning => node.Warning == warning)).ToList();
            foreach (var node in toRemove) {
                RemoveNode(node);
            }
            
            // Generate list of warnings we need to add
            var toAdd = warningStates.Where(warning => !nodeList.Any(node => node.Warning == warning)).ToList();
            foreach (var warning in toAdd) {
                AddWarning(warning);
            }
        }
    }

    private void AddWarning(WarningState warning) {
        if (BannerListNode is null) return;
        if (NodeListHasWarning(warning)) return;
        
        AddNode(warning);
    }

    private bool NodeListHasWarning(WarningState warning)
        => nodeList.Any(node => node.Warning == warning);

    public void PlayAnimation(int label)
        => BannerListNode?.Timeline?.PlayAnimation(label);
}
