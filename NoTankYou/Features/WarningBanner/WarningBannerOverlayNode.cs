using System.Linq;
using System.Numerics;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit;
using KamiToolKit.Enums;
using KamiToolKit.Nodes;
using KamiToolKit.Overlay;
using KamiToolKit.Timelines;
using NoTankYou.Classes;
using NoTankYou.Enums;

namespace NoTankYou.Features.WarningBanner;

public class WarningBannerOverlayNode : OverlayNode {
	public override OverlayLayer OverlayLayer => OverlayLayer.Background;

	private readonly ListNode<WarningInfo, WarningBannerListItemNode> bannerListNode;
    
    public required WarningBannerConfig Config { get; init; }
    
	public WarningBannerOverlayNode() {
		bannerListNode = new ListNode<WarningInfo, WarningBannerListItemNode> {
            ItemSpacing = 8.0f,
			DisableCollisionNode = true,
            OptionsList = [],
		};
		bannerListNode.AttachNode(this);
		
		bannerListNode.AddTimeline(new TimelineBuilder()
          .BeginFrameSet(1, 60)
          .AddLabel(1, 1, AtkTimelineJumpBehavior.Start, 0) // Label 1: Pulsing Animation
          .AddLabel(30, 0, AtkTimelineJumpBehavior.LoopForever, 1)
          .AddLabel(31, 2, AtkTimelineJumpBehavior.Start, 0) // Label 2: No Animation
          .AddLabel(60, 0, AtkTimelineJumpBehavior.LoopForever, 2)
          .EndFrameSet()
          .Build());
        
        OnEditComplete += UpdateSizePosition;
	}

	protected override void OnSizeChanged() {
		base.OnSizeChanged();

		bannerListNode.Size = Size;
	}

	protected override void OnUpdate() {
        EnableMoving = Config.EnableMoving;
        EnableResizing = Config.EnableResizing;

        Scale = new Vector2(Config.Scale, Config.Scale);

        var filteredModules = System.WarningController.ActiveWarnings
            .Where(module => !Config.BlacklistedModules.Contains(module.SourceModule))
            .ToList();

        switch (Config.DisplayMode) {
            case BannerDisplayMode.TopPriority when filteredModules.Count is not 0:
                bannerListNode.OptionsList = [ filteredModules.First() ];
                break;
            
            case BannerDisplayMode.TopPriority when filteredModules.Count is 0:
                bannerListNode.OptionsList = [];
                break;
            
            case BannerDisplayMode.List:
                bannerListNode.OptionsList = filteredModules;
                break;
        }

        bannerListNode.Timeline?.PlayAnimation(Config.EnableAnimation ? 1 : 2);
    }

    private void UpdateSizePosition(NodeBase nodeBase) {
        Config.Position = Position;
        Config.Size = Size;
        Config.MarkDirty();
    }
}
