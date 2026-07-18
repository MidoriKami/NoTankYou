using System.Numerics;
using System.Threading.Tasks;
using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Client.UI;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Interfaces;
using KamiToolKit.Nodes;
using NoTankYou.Classes;
using NoTankYou.Enums;

namespace NoTankYou.CustomNodes;

public class ModuleTreeListItemNode : TreeListItemNode<LoadedModule>, ITreeListItemNode {

    public static float ItemHeight => 32.0f;

    protected override unsafe void SetNodeData(LoadedModule itemData) {
        labelTextNode.String = itemData.FeatureBase.Name;

        checkboxNode.OnClick = null;
        checkboxNode.IsEnabled = itemData.State is not LoadedState.Errored;
        checkboxNode.IsChecked = itemData.State is LoadedState.Enabled;
        checkboxNode.OnClick = shouldEnable => Task.Run(() => ToggleModification(shouldEnable));

        if (itemData.FeatureBase.ModuleInfo.IconId is 0) {
            iconImageNode.IsVisible = false;
        }
        else {
            iconImageNode.IconId = itemData.FeatureBase.ModuleInfo.IconId;
        }

        var parentAddon = RaptureAtkUnitManager.Instance()->GetAddonByNode(this);
        if (parentAddon is not null) {
            parentAddon->UpdateCollisionNodeList(false);
        }

        checkboxNode.IsChecked = itemData.State is LoadedState.Enabled;
    }

    public ModuleTreeListItemNode() {
        checkboxNode = new CheckboxNode {
            OnClick = newState => Task.Run(() => ToggleModification(newState)),
        };
        checkboxNode.AttachNode(this);

        iconImageNode = new IconImageNode {
            FitTexture = true,
        };
        iconImageNode.AttachNode(this);

        labelTextNode = new TextNode {
            TextFlags = TextFlags.Ellipsis,
        };
        labelTextNode.AttachNode(this);
    }

    protected override void OnSizeChanged() {
        base.OnSizeChanged();

        checkboxNode.Size = new Vector2(Height, Height) * 3.0f / 4.0f;
        checkboxNode.Position = new Vector2(Height, Height) / 8.0f;

        iconImageNode.Size = new Vector2(Height - 4.0f, Height - 4.0f);
        iconImageNode.Position = new Vector2(checkboxNode.Bounds.Right + 6.0f, 2.0f);

        labelTextNode.Size = new Vector2(Width - iconImageNode.Bounds.Right - 4.0f, Height);
        labelTextNode.Position = new Vector2(iconImageNode.Bounds.Right + 4.0f, 0.0f);

        if (!iconImageNode.IsVisible) {
            labelTextNode.X -= iconImageNode.Width;
            labelTextNode.Width += iconImageNode.Width;
        }
    }

    private async Task ToggleModification(bool shouldEnableModification) {
        if (ItemData is null) return;

        if (shouldEnableModification && ItemData.State is LoadedState.Disabled) {
            await ModuleManager.TryEnableModule(ItemData);
        }
        else if (!shouldEnableModification && ItemData.State is LoadedState.Enabled) {
            await ModuleManager.TryDisableModification(ItemData);
        }

        unsafe {
            var parentAddon = RaptureAtkUnitManager.Instance()->GetAddonByNode(this);
            if (parentAddon is not null) {
                parentAddon->UpdateCollisionNodeList(false);
            }

            checkboxNode.IsChecked = ItemData.State is LoadedState.Enabled;
        }

        await IFramework.Get().Run(() => OnClick?.Invoke(this));
    }

    private readonly CheckboxNode checkboxNode;
    private readonly IconImageNode iconImageNode;
    private readonly TextNode labelTextNode;
}
