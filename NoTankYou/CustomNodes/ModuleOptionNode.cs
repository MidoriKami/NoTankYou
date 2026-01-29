using System.Numerics;
using FFXIVClientStructs.FFXIV.Client.UI;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit;
using KamiToolKit.Nodes;
using NoTankYou.Classes;
using NoTankYou.Enums;

namespace NoTankYou.CustomNodes;

public class ModuleOptionNode : SelectableNode {
    private readonly CheckboxNode checkboxNode;
    private readonly IconImageNode iconImageNode;
    private readonly TextNode moduleNameNode;

    public ModuleOptionNode() {
        checkboxNode = new CheckboxNode {
            OnClick = ToggleModification,
        };
        checkboxNode.AttachNode(this);

        iconImageNode = new IconImageNode {
            FitTexture = true,
        };
        iconImageNode.AttachNode(this);

        moduleNameNode = new TextNode {
            TextFlags = TextFlags.Ellipsis,
            AlignmentType = AlignmentType.Left,
            FontSize = 14,
        };
        moduleNameNode.AttachNode(this);

        CollisionNode.ShowClickableCursor = true;
    }

    public ModuleInfo ModuleInfo => Module.FeatureBase.ModuleInfo;
    
    public required LoadedModule Module {
        get;
        set {
            field = value;
            moduleNameNode.String = value.FeatureBase.Name;
            
            checkboxNode.IsChecked = value.State is LoadedState.Enabled;

            if (value.FeatureBase.ModuleInfo.IconId is 0) {
                iconImageNode.IsVisible = false;
            }
            else {
                iconImageNode.IconId = value.FeatureBase.ModuleInfo.IconId;
            }

            UpdateDisabledState();
        }
    }
    
    private void ToggleModification(bool shouldEnableModification) {
        if (shouldEnableModification && Module.State is LoadedState.Disabled) {
            ModuleManager.TryEnableModule(Module);
        }
        else if (!shouldEnableModification && Module.State is LoadedState.Enabled) {
            ModuleManager.TryDisableModification(Module);
        }

        UpdateDisabledState();
        
        OnClick?.Invoke(this);
    }

    protected override void OnSizeChanged() {
        base.OnSizeChanged();

        checkboxNode.Size = new Vector2(Height, Height) * 3.0f / 4.0f;
        checkboxNode.Position = new Vector2(Height, Height) / 8.0f;
        
        iconImageNode.Size = new Vector2(Height - 4.0f, Height - 4.0f);
        iconImageNode.Position = new Vector2(checkboxNode.Bounds.Right + 6.0f, 2.0f);

        moduleNameNode.Size = new Vector2(Width - iconImageNode.Bounds.Right - 4.0f, Height);
        moduleNameNode.Position = new Vector2(iconImageNode.Bounds.Right + 4.0f, 0.0f);

        if (!iconImageNode.IsVisible) {
            moduleNameNode.X -= iconImageNode.Width;
            moduleNameNode.Width += iconImageNode.Width;
        }
    }

    private void UpdateDisabledState() {
        UpdateCollisionForNode(this);

        checkboxNode.IsChecked = Module.State is LoadedState.Enabled;
    }

    private static unsafe void UpdateCollisionForNode(NodeBase node) {
        var addon = RaptureAtkUnitManager.Instance()->GetAddonByNode(node);
        if (addon is not null) {
            addon->UpdateCollisionNodeList(false);
        }
    }
}
