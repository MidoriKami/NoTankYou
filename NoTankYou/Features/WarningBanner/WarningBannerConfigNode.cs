using KamiToolKit.Nodes;
using NoTankYou.Features.WarningBanner.ConfigurationCategories;

namespace NoTankYou.Features.WarningBanner;

public class WarningBannerConfigNode : ResNode {
    private readonly ScrollingNode<VerticalListNode> listNode;

    public WarningBannerConfigNode(WarningBanner module) {
        listNode = new ScrollingNode<VerticalListNode> {
            ContentNode = {
                FitWidth = true,
                FitContents = true,
                ItemSpacing = 3.0f,
                InitialNodes = [
                    new FeatureConfigurationNode(module),
                    new PositioningConfigurationNode(module),
                    new DisplayStyleConfigNode(module),
                    new ModuleSelectConfigNode(module),
                ],
            },
        };
        listNode.AttachNode(this);
    }

    protected override void OnSizeChanged() {
        base.OnSizeChanged();

        listNode.Size = Size;
        listNode.RecalculateSizes();
    }
}
