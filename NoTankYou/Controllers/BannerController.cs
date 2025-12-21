using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using NoTankYou.Classes;
using KamiToolKit.Overlay;

namespace NoTankYou.Controllers;

public class BannerController : IDisposable {
    private BannerOverlayNode? overlayNode;
    private BannerNode? sampleNode;

    private readonly OverlayController overlayController;

    public BannerController() {
        overlayController = new OverlayController();
        
        overlayController.CreateNode(() => overlayNode = new BannerOverlayNode {
            Position = new Vector2(700.0f, 400.0f),
            Size = new Vector2(448.0f, 320.0f),
        });
    }

    public void Dispose()
        => overlayController.Dispose();
    
    public void UpdateWarnings(List<WarningState> warningStates) {
        if (overlayNode is null) return;
        if (System.BannerConfig is not {} config) return;
        if (!config.Enabled) {
            RemoveAllNodes();
            return;
        }
        
        var filteredWarningStates = warningStates
            .Where(warning => !config.BlacklistedModules.Contains(warning.SourceModule))
            .OrderByDescending(warning => warning.Priority)
            .ToList();

        if (config.SoloMode) {
            filteredWarningStates = filteredWarningStates
                .Where(warning => warning.SourceEntityId == Services.ObjectTable.LocalPlayer?.EntityId)
                .ToList();
        }

        if (config.SampleMode) {
            sampleNode ??= new BannerNode {
                Size = new Vector2(448.0f, 64.0f), 
            };
            
            filteredWarningStates.Add(ModuleController.SampleWarning);
        }

        switch (config.DisplayMode) {
            case BannerOverlayDisplayMode.TopPriority when filteredWarningStates.MaxBy(warning => warning.Priority) is { } topWarning:
                SyncWarnings([ topWarning ]);
                break;

            case BannerOverlayDisplayMode.List:
                SyncWarnings(filteredWarningStates);
                break;
            
            default:
                SyncWarnings([]);
                break;
        }
    }

    private void AddNode(WarningState warningState) {
        var newBannerNode = new BannerNode {
            Size = new Vector2(448.0f, 64.0f),
            IsVisible = true,
        };

        newBannerNode.Warning = warningState;
        overlayNode?.AddNode(newBannerNode);
    }

    private void RemoveAllNodes() {
        overlayNode?.RemoveAll();
    }
    
    private void RemoveNode(BannerNode node) {
        node.HideTooltip();
        overlayNode?.RemoveNode(node);
    }

    private void SyncWarnings(List<WarningState> warningStates) {
        if (overlayNode is null) return;
        
        if (warningStates.Count is 0) {
            RemoveAllNodes();
        }
        else {
            // Get a list of nodes that need to be removed
            var toRemove = overlayNode.Nodes.Where(node => !warningStates.Any(warning => node.Warning == warning)).ToList();
            foreach (var node in toRemove) {
                RemoveNode(node);
            }
            
            // Generate list of warnings we need to add
            var toAdd = warningStates.Where(warning => !overlayNode.Nodes.Any(node => node.Warning == warning)).ToList();
            foreach (var warning in toAdd) {
                AddWarning(warning);
            }
        }
    }

    private void AddWarning(WarningState warning) {
        if (overlayNode is null) return;
        if (NodeListHasWarning(warning)) return;
        
        AddNode(warning);
    }

    private bool NodeListHasWarning(WarningState warning)
        => overlayNode?.Nodes.Any(node => node.Warning == warning) ?? false;
}
