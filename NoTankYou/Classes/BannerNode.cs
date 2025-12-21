using System.Drawing;
using System.Numerics;
using Dalamud.Interface;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Classes.Timelines;
using KamiToolKit.Nodes;

namespace NoTankYou.Classes;

public sealed class BannerNode : SimpleComponentNode {
	private readonly IconImageNode warningImageNode;
	private readonly TextNode messageTextNode;
	private readonly TextNode playerTextNode;
	private readonly IconImageNode actionIconNode;
	private readonly TextNode actionNameNode;
	private readonly TextNode helpTextNode;
	
	public BannerNode() {
		warningImageNode = new IconImageNode {
			IconId = 230424,
			FitTexture = true,
		};
		warningImageNode.AttachNode(this);

		messageTextNode = new TextNode {
			TextColor = KnownColor.White.Vector(),
			TextOutlineColor = KnownColor.Black.Vector(),
			FontSize = 26,
			FontType = FontType.Axis, 
			TextFlags = TextFlags.Edge | TextFlags.Ellipsis,
			AlignmentType = AlignmentType.Left,
			String = "Long Default Message Text",
		};
		messageTextNode.AttachNode(this);

		playerTextNode = new TextNode {
			TextColor = KnownColor.White.Vector() with { W = 0.66f },
			TextOutlineColor = KnownColor.Black.Vector() with { W = 0.33f },
			FontSize = 18,
			FontType = FontType.Axis, 
			TextFlags = TextFlags.Edge | TextFlags.Ellipsis,
			AlignmentType = AlignmentType.Left,
			String = "PlayerName Here",
		};
		playerTextNode.AttachNode(this);

		actionIconNode = new IconImageNode {
			IconId = 61502,
			FitTexture = true,
		};
		actionIconNode.AttachNode(this);

		actionNameNode = new TextNode {
			TextColor = KnownColor.White.Vector(),
			TextOutlineColor = KnownColor.Black.Vector(),
			FontSize = 12,
			FontType = FontType.Axis, 
			TextFlags = TextFlags.Edge | TextFlags.Ellipsis,
			AlignmentType = AlignmentType.Top,
			String = "Action Name",
		};
		actionNameNode.AttachNode(this);

		helpTextNode = new TextNode {
			TextColor = KnownColor.White.Vector(),
			TextOutlineColor = KnownColor.Black.Vector(),
			FontSize = 16,
			FontType = FontType.Axis,
			TextFlags = TextFlags.Edge,
			AlignmentType = AlignmentType.Center,
			String = "?",
			Tooltip = "Overlay from NoTankYou plugin",
		};
		helpTextNode.AttachNode(this);

		actionIconNode.AddFlags(NodeFlags.HasCollision);
		actionIconNode.AddEvent(AtkEventType.MouseOver, OnIconMouseOver);
		actionIconNode.AddEvent(AtkEventType.MouseOut, actionIconNode.HideTooltip);
		
		AddTimeline(new TimelineBuilder()
            .BeginFrameSet(1, 30)
            .AddFrame(1, scale: new Vector2(0.95f, 0.95f), alpha: 175)
            .AddFrame(10, scale: new Vector2(0.95f, 0.95f), alpha: 175)
            .AddFrame(15, scale: new Vector2(1.0f, 1.0f), alpha: 255)
            .AddFrame(25, scale: new Vector2(0.95f, 0.95f), alpha: 175)
            .AddFrame(30, scale: new Vector2(0.95f, 0.95f), alpha: 175)
            .EndFrameSet()
            .BeginFrameSet(31, 60)
            .AddFrame(31, alpha: 255, scale: new Vector2(1.0f, 1.0f))
            .EndFrameSet()
            .Build());
	}

	private void OnIconMouseOver() {
		if (Warning is not { ActionId: not 0 }) return;
		if (System.BannerStyle is not { EnableActionTooltip: true }) return;

		actionIconNode.ShowActionTooltip(Warning.ActionId);
	}

	public override float Height {
		get => base.Height;
		set {
			base.Height = value;
			warningImageNode.Height = value;
			messageTextNode.Height = value / 2.0f;
			playerTextNode.Height = value / 2.0f;
			playerTextNode.Y = value / 2.0f;
			actionIconNode.Height = value * 3.0f / 4.0f;
			actionIconNode.Width = value * 3.0f / 4.0f;
			actionNameNode.X = actionIconNode.X - actionNameNode.Width / 2.0f + actionIconNode.Width / 2.0f;
			actionNameNode.Height = value / 3.0f;
			actionNameNode.Y = actionIconNode.Height;
			helpTextNode.Height = 16.0f;
			helpTextNode.Y = value / 2.0f - helpTextNode.Height / 2.0f;
			OriginY = value / 2.0f;
		}
	}

	public override float Width {
		get => base.Width;
		set {
			base.Width = value;
			warningImageNode.Width = value / 7.0f;
			messageTextNode.Width = value * 5.0f / 7.0f;
			messageTextNode.X = value / 7.0f;
			playerTextNode.Width = value * 5.0f / 7.0f;
			playerTextNode.X = value / 7.0f;
			actionIconNode.X = value * 6.0f / 7.0f;
			actionNameNode.Width = value * 2.0f / 7.0f;
			actionNameNode.X = actionIconNode.X - actionNameNode.Width / 2.0f + actionIconNode.Width / 2.0f;
			helpTextNode.Width = 16.0f;
			helpTextNode.X = value - helpTextNode.Width;
			OriginX = value / 2.0f;
		}
	}

	public WarningState? Warning {
		get;
		set {
			field = value;

			if (value is null) {
				IsVisible = false;
			}
			else {
				messageTextNode.String = value.Message;
				playerTextNode.String = value.SourcePlayerName;
				actionIconNode.IconId = value.IconId;
				actionNameNode.String = value.IconLabel;
			}
		}
	}

	public bool ShowWarningImage {
		get => warningImageNode.IsVisible;
		set => warningImageNode.IsVisible = value;
	}

	public bool ShowMessageText {
		get => messageTextNode.IsVisible;
		set => messageTextNode.IsVisible = value;
	}

	public bool ShowPlayerText {
		get => playerTextNode.IsVisible;
		set => playerTextNode.IsVisible = value;
	}

	public bool ShowActionIcon {
		get => actionIconNode.IsVisible;
		set => actionIconNode.IsVisible = value;
	}

	public bool ShowActionName {
		get => actionNameNode.IsVisible;
		set => actionNameNode.IsVisible = value;
	}
}