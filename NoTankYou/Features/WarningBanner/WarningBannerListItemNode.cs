using System.Drawing;
using System.Numerics;
using Dalamud.Interface;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Nodes;
using KamiToolKit.Timelines;
using NoTankYou.Classes;

namespace NoTankYou.Features.WarningBanner;

public sealed unsafe class WarningBannerListItemNode : ListItemNode<WarningInfo> {
    public override float ItemHeight => 75.0f;

    private readonly IconImageNode warningImageNode;
	private readonly TextNode messageTextNode;
	private readonly TextNode playerTextNode;
	private readonly IconImageNode actionIconNode;
	private readonly TextNode actionNameNode;

    public WarningBannerListItemNode() {
        DisableInteractions();

        warningImageNode = new IconImageNode {
			IconId = 230424,
			FitTexture = true,
		};
		warningImageNode.AttachNode(this);

		messageTextNode = new TextNode {
			TextColor = KnownColor.White.Vector(),
			TextOutlineColor = KnownColor.Black.Vector(),
			FontSize = 26,
			TextFlags = TextFlags.Edge | TextFlags.Ellipsis,
			AlignmentType = AlignmentType.BottomLeft,
		};
		messageTextNode.AttachNode(this);

		playerTextNode = new TextNode {
			TextColor = KnownColor.White.Vector() with { W = 0.66f },
			TextOutlineColor = KnownColor.Black.Vector() with { W = 0.33f },
			FontSize = 18,
			TextFlags = TextFlags.Edge | TextFlags.Ellipsis,
			AlignmentType = AlignmentType.Bottom,
		};
		playerTextNode.AttachNode(this);

        actionNameNode = new TextNode {
            TextColor = KnownColor.White.Vector(),
			TextOutlineColor = KnownColor.Black.Vector() with { W = 0.33f },
            FontSize = 12,
            TextFlags = TextFlags.Edge | TextFlags.Ellipsis,
            AlignmentType = AlignmentType.Top,
        };
        actionNameNode.AttachNode(this);
        
		actionIconNode = new IconImageNode {
			FitTexture = true,
            ActionTooltip = 1,
		};
		actionIconNode.AttachNode(this);

        AddTimeline(new TimelineBuilder()
            .BeginFrameSet(1, 30)
            .AddFrame(1, scale: new Vector2(1.00f, 1.00f), alpha: 175)
            .AddFrame(10, scale: new Vector2(1.00f, 1.00f), alpha: 175)
            .AddFrame(15, scale: new Vector2(1.05f, 1.05f), alpha: 255)
            .AddFrame(25, scale: new Vector2(1.00f, 1.00f), alpha: 175)
            .AddFrame(30, scale: new Vector2(1.00f, 1.00f), alpha: 175)
            .EndFrameSet()
            .BeginFrameSet(31, 60)
            .AddFrame(31, alpha: 255, scale: new Vector2(1.0f, 1.0f))
            .EndFrameSet()
            .Build());
	}

    protected override void OnSizeChanged() {
        base.OnSizeChanged();

        Origin = Bounds.Center;
        
        warningImageNode.Size = new Vector2(Height - 4.0f, Height - 4.0f);
        warningImageNode.Position = new Vector2(2.0f, 2.0f);

        messageTextNode.Size = new Vector2(Width - Height * 2.0f - 4.0f, Height * 4.0f / 10.0f);
        messageTextNode.Position = new Vector2(warningImageNode.Bounds.Right + 2.0f, 0.0f);
        
        playerTextNode.Size = new Vector2(Width - Height * 2.0f - 4.0f, Height * 3.0f / 10.0f);
        playerTextNode.Position = new Vector2(warningImageNode.Bounds.Right + 2.0f, Height * 4.0f / 10.0f);
        
        actionNameNode.Size = new Vector2(Width - Height * 2.0f - 4.0f, Height * 3.0f / 10.0f);
        actionNameNode.Position = new Vector2(warningImageNode.Bounds.Right + 2.0f, Height * 7.0f / 10.0f);

        actionIconNode.Size = new Vector2(Height - 4.0f, Height - 4.0f);
        actionIconNode.Position = new Vector2(Width - actionIconNode.Width - 2.0f, 2.0f);
    }

    protected override void SetNodeData(WarningInfo itemData) {
        messageTextNode.String = itemData.Message;
        playerTextNode.String = itemData.SourceCharacter->GetName().ToString();
        actionIconNode.IconId = itemData.IconId;
        actionNameNode.String = itemData.IconLabel;

        if (WarningBanner.WarningBannerConfig is not { } config) return;
        
        warningImageNode.IsVisible = config.ShowWarningShield;
        messageTextNode.IsVisible = config.ShowWarningText;
        playerTextNode.IsVisible = config.ShowPlayerName;
        actionNameNode.IsVisible = config.ShowActionName;
        actionIconNode.IsVisible = config.ShowActionIcon;

        actionIconNode.ActionTooltip = config.EnableActionTooltip ? itemData.ActionId : 0;
    }
}
