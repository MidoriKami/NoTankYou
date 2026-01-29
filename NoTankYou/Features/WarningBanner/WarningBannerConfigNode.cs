using KamiToolKit.Nodes;
using NoTankYou.Features.WarningBanner.ConfigurationCategories;

namespace NoTankYou.Features.WarningBanner;

public class WarningBannerConfigNode : SimpleComponentNode {
    private readonly ScrollingListNode listNode;

    public WarningBannerConfigNode(WarningBanner module) {
        listNode = new ScrollingListNode {
            FitWidth = true,
            ItemSpacing = 3.0f,
            InitialNodes = [
                new FeatureConfigurationNode(module),
                new PositioningConfigurationNode(module),
                new DisplayStyleConfigNode(module),
                new ModuleSelectConfigNode(module),
            ],
        };
        listNode.AttachNode(this);
    }

    protected override void OnSizeChanged() {
        base.OnSizeChanged();

        listNode.Size = Size;
        listNode.RecalculateLayout();
    }
}
