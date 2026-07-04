using KamiToolKit.Nodes;
using NoTankYou.Features.PartyList.ConfigurationCategories;

namespace NoTankYou.Features.PartyList;

public class PartyListConfigNode : ResNode {
    private readonly ScrollingNode<VerticalListNode> listNode;

    public PartyListConfigNode(PartyList module) {
        listNode = new ScrollingNode<VerticalListNode> {
            ContentNode = {
                FitWidth = true,
                FitContents = true,
                ItemSpacing = 3.0f,
                InitialNodes = [
                    new FeatureConfigurationNode(module),
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
