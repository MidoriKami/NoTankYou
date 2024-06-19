using System.Drawing;
using System.Numerics;
using Dalamud.Interface;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Classes;
using KamiToolKit.Nodes;

namespace NoTankYou.Classes;

public class BannerOverlayNode : NodeBase<AtkResNode> {

	private readonly ImageNode warningImageNode;
	private readonly TextNode messageTextNode;
	private readonly TextNode playerTextNode;
	private readonly ImageNode actionIconNode;
	private readonly TextNode actionNameNode;

	private WarningState? internalWarning;
	
	public WarningState? AssociatedWarning {
		get => internalWarning;
		set => SetWarning(value);
	}

	public BannerOverlayNode(uint nodeId) : base(NodeType.Res) {
		NodeID = nodeId;
		Width = 64.0f + 300.0f + 48.0f + 64.0f;
		Height = 64.0f;
		IsVisible = true;
		
		warningImageNode = new ImageNode {
			NodeID = 200000 + nodeId,
			Position = Vector2.Zero,
			Size = new Vector2(64.0f, 64.0f),
			IsVisible = true,
		};
		
		warningImageNode.LoadIcon(76579);
		warningImageNode.AttachNode(this, NodePosition.AsLastChild);

		messageTextNode = new TextNode {
			NodeID = 210000 + nodeId,
			Position = new Vector2(64.0f, 0.0f),
			Size = new Vector2(300.0f, 32.0f),
			TextColor = KnownColor.White.Vector(),
			OutlineColor = KnownColor.Black.Vector(),
			IsVisible = true,
			FontSize = 26,
			FontType = FontType.Axis, 
			TextFlags = TextFlags.Edge,
			TextFlags2 = TextFlags2.Ellipsis,
			Text = "Warning Will Robinson This Is Super Long!",
		};
		
		messageTextNode.AttachNode(this, NodePosition.AsLastChild);

		playerTextNode = new TextNode {
			NodeID = 220000 + nodeId,
			Position = new Vector2(64.0f, 32.0f),
			Size = new Vector2(300.0f, 32.0f),
			TextColor = KnownColor.White.Vector() with { W = 0.66f },
			OutlineColor = KnownColor.Black.Vector() with { W = 0.33f },
			IsVisible = true,
			FontSize = 18,
			FontType = FontType.Axis, 
			TextFlags = TextFlags.Edge,
			TextFlags2 = TextFlags2.Ellipsis,
			Text = "Dumb Player Name",
		};

		playerTextNode.AttachNode(this, NodePosition.AsLastChild);

		actionIconNode = new ImageNode {
			NodeID = 230000 + nodeId,
			Position = new Vector2(300.0f + 64.0f + 32.0f, 0.0f),
			Size = new Vector2(48.0f, 48.0f),
			IsVisible = true,
		};
		
		actionIconNode.LoadIcon(61502);
		actionIconNode.AttachNode(this, NodePosition.AsLastChild);

		actionNameNode = new TextNode {
			NodeID = 240000 + nodeId,
			Position = new Vector2(300.0f + 64.0f, 32.0f),
			Size = new Vector2(64.0f + 48.0f, 48.0f),
			TextColor = KnownColor.White.Vector(),
			OutlineColor = KnownColor.Black.Vector(),
			IsVisible = true,
			FontSize = 12,
			FontType = FontType.Axis, 
			TextFlags = TextFlags.Edge,
			TextFlags2 = TextFlags2.Ellipsis,
			AlignmentType = AlignmentType.Center,
			Text = "Action Name",
		};
		
		actionNameNode.AttachNode(this, NodePosition.AsLastChild);
	}

	private void SetWarning(WarningState? warning) {
		if (warning is null) {
			IsVisible = false;
			return;
		}
		
		if (internalWarning != warning) {
			internalWarning = warning;

			messageTextNode.Text = warning.Message;
			playerTextNode.Text = warning.SourcePlayerName;
			actionIconNode.LoadIcon(warning.IconId);
			actionNameNode.Text = warning.IconLabel;
		}
	}

	public void UpdateStyle() {
		warningImageNode.IsVisible = System.BannerController.Config.WarningShield;
		messageTextNode.IsVisible = System.BannerController.Config.WarningText;
		playerTextNode.IsVisible = System.BannerController.Config.PlayerNames;
		actionIconNode.IsVisible = System.BannerController.Config.Icon;
		actionNameNode.IsVisible = System.BannerController.Config.ShowActionName;
	}
}