using System.Collections.Generic;
using System.Drawing;
using System.Numerics;
using Dalamud.Interface;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiLib.Configuration;
using KamiToolKit.Classes.TimelineBuilding;
using KamiToolKit.Nodes;
using Newtonsoft.Json;

namespace NoTankYou.Classes;

[JsonObject(MemberSerialization.OptIn)]
public sealed class BannerOverlayNode : SimpleComponentNode {

	[JsonProperty] private readonly IconImageNode warningImageNode;
	[JsonProperty] private readonly TextNode messageTextNode;
	[JsonProperty] private readonly TextNode playerTextNode;
	[JsonProperty] private readonly IconImageNode actionIconNode;
	[JsonProperty] private readonly TextNode actionNameNode;
	[JsonProperty] private readonly TextNode helpTextNode;

    private static string WarningNodePath => Service.PluginInterface.GetCharacterFileInfo(Service.ClientState.LocalContentId, "BannerNode.style.json").FullName;
	
	public BannerOverlayNode() {
		warningImageNode = new IconImageNode {
			NodeId = 2,
			IsVisible = true,
			IconId = 230424,
		};
		System.NativeController.AttachNode(warningImageNode, this);

		messageTextNode = new TextNode {
			NodeId = 3,
			TextColor = KnownColor.White.Vector(),
			TextOutlineColor = KnownColor.Black.Vector(),
			IsVisible = true,
			FontSize = 26,
			FontType = FontType.Axis, 
			TextFlags = TextFlags.Edge,
			TextFlags2 = TextFlags2.Ellipsis,
			AlignmentType = AlignmentType.Left,
			Text = "Long Default Message Text",
		};
		System.NativeController.AttachNode(messageTextNode, this);

		playerTextNode = new TextNode {
			NodeId = 4,
			TextColor = KnownColor.White.Vector() with { W = 0.66f },
			TextOutlineColor = KnownColor.Black.Vector() with { W = 0.33f },
			IsVisible = true,
			FontSize = 18,
			FontType = FontType.Axis, 
			TextFlags = TextFlags.Edge,
			TextFlags2 = TextFlags2.Ellipsis,
			AlignmentType = AlignmentType.Left,
			Text = "PlayerName Here",
		};
		System.NativeController.AttachNode(playerTextNode, this);

		actionIconNode = new IconImageNode {
			NodeId = 5,
			IsVisible = true,
			IconId = 61502,
		};
		System.NativeController.AttachNode(actionIconNode, this);

		actionNameNode = new TextNode {
			NodeId = 6,
			TextColor = KnownColor.White.Vector(),
			TextOutlineColor = KnownColor.Black.Vector(),
			IsVisible = true,
			FontSize = 12,
			FontType = FontType.Axis, 
			TextFlags = TextFlags.Edge,
			TextFlags2 = TextFlags2.Ellipsis,
			AlignmentType = AlignmentType.Top,
			Text = "Action Name",
		};
		System.NativeController.AttachNode(actionNameNode, this);

		helpTextNode = new TextNode {
			NodeId = 7,
			TextColor = KnownColor.White.Vector(),
			TextOutlineColor = KnownColor.Black.Vector(),
			IsVisible = true,
			FontSize = 16,
			FontType = FontType.Axis,
			TextFlags = TextFlags.Edge,
			AlignmentType = AlignmentType.Center,
			EnableEventFlags = true,
			Text = "?",
			Tooltip = "Overlay from NoTankYou plugin",
		};
		System.NativeController.AttachNode(helpTextNode, this);

		BuildTimelines();
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
				messageTextNode.Text = value.Message;
				playerTextNode.Text = value.SourcePlayerName;
				actionIconNode.IconId = value.IconId;
				actionNameNode.Text = value.IconLabel;
			}
		}
	}

	protected override List<string> OnLoadOmittedProperties => [ "Position" ];

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

	public void Save()
		=> Save(WarningNodePath);
	
	public void Load()
		=> Load(WarningNodePath);
	
	
	private void BuildTimelines() {
		AddTimeline(new TimelineBuilder()
			.BeginFrameSet(1, 30)
			.AddFrame(1, scale: new Vector2(0.95f, 0.95f), alpha: 150)
			.AddFrame(15, scale: new Vector2(1.0f, 1.0f), alpha: 255)
			.AddFrame(30, scale: new Vector2(0.95f, 0.95f), alpha: 150)
			.EndFrameSet()
			.BeginFrameSet(31, 60)
			.AddFrame(31, alpha: 255, scale: new Vector2(1.0f, 1.0f))
			.EndFrameSet()
			.Build());
	}
}