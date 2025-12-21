using System.Collections.Immutable;
using System.Drawing;
using System.Numerics;
using Dalamud.Interface;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Classes;
using KamiToolKit.Classes.Timelines;
using KamiToolKit.Nodes;
using KamiToolKit.Overlay;

namespace NoTankYou.Classes;

public class BannerOverlayNode : OverlayNode {
	public override OverlayLayer OverlayLayer => OverlayLayer.Background;

	private readonly ListBoxNode bannerListNode;

	public BannerOverlayNode() {
		bannerListNode = new ListBoxNode {
			LayoutAnchor = LayoutAnchor.TopLeft,
			LayoutOrientation = LayoutOrientation.Vertical,
			BackgroundColor = KnownColor.Aqua.Vector() with { W = 0.15f },
			ShowBackground = false,
			ClipListContents = true,
			OnEditComplete = UpdateSizePosition,
			DisableCollisionNode = true,
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
	}

	protected override void OnSizeChanged() {
		base.OnSizeChanged();

		bannerListNode.Size = Size;
	}

	public void AddNode(BannerNode node) {
		bannerListNode.AddNode(node);
	}
	
	public void RemoveNode(BannerNode node) {
		bannerListNode.RemoveNode(node);
	}

	public void RemoveAll() {
		bannerListNode.Clear();
	}

	public ImmutableList<BannerNode> Nodes => bannerListNode.GetNodes<BannerNode>().ToImmutableList();

	public override void Update() {
		base.Update();

		if (System.BannerConfig is { } config) {
			IsVisible = config.Enabled;
		}

		if (System.BannerListStyle is { } listConfig) {
			if (Position == Vector2.Zero) {
				Position = listConfig.Position;
			}

			EnableMoving = listConfig.EnableMoving;
			EnableResizing = listConfig.EnableResizing;
		
			bannerListNode.LayoutOrientation = listConfig.Orientation;
			bannerListNode.LayoutAnchor = listConfig.Anchor;
			bannerListNode.ShowBackground = listConfig.ShowBackground;
			bannerListNode.BackgroundColor = listConfig.BackgroundColor;
		}

		if (System.BannerStyle is { } style) {
			bannerListNode.Timeline?.PlayAnimation(style.EnableAnimation ? 1 : 2);
			
			foreach (var node in Nodes) {
				node.ShowWarningImage = style.ShowWarningIcon;
				node.ShowMessageText = style.ShowMessageText;
				node.ShowPlayerText = style.ShowPlayerText;
				node.ShowActionName = style.ShowActionName;
				node.ShowActionIcon = style.ShowActionIcon;
			}
		}
	}
	
	private void UpdateSizePosition() {
		if (System.BannerListStyle is not {} config) return;

		var configChanged = Position != config.Position || Size != config.Size;

		config.Position = Position;
		config.Size = Size;

		if (configChanged) {
			Utilities.Config.SaveCharacterConfig(config, "BannerList.style.json");
		}
	}
}