using System.Numerics;
using Dalamud.Game.Text.SeStringHandling;
using FFXIVClientStructs.FFXIV.Client.Game.UI;
using FFXIVClientStructs.FFXIV.Client.UI;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiLib.Configuration;
using KamiToolKit.Classes.TimelineBuilding;
using KamiToolKit.NodeParts;
using KamiToolKit.Nodes;

namespace NoTankYou.Classes;

public unsafe class PartyListOverlayNode : SimpleOverlayNode {

	private readonly IconImageNode warningIconImageNode;
	private readonly IconImageNode iconDecoratorImageNode;
	private readonly TextNode nameTextNode;

	private Vector4 defaultOutlineColor = new Vector4(8.0f, 80.0f, 152.0f, 255.0f) / 255.0f;
	
    private static string PartyListNodePath => Service.PluginInterface.GetCharacterFileInfo(Service.ClientState.LocalContentId, "PartyListNode.style.json").FullName;

	public PartyListOverlayNode() {
		warningIconImageNode = new IconImageNode {
			NodeId = 3,
			Size = new Vector2(40.0f, 40.0f),
			Position = new Vector2(20.0f, 14.0f),
			IconId = 60074,
			WrapMode = 1,
			IsVisible = true,
		};
		System.NativeController.AttachNode(warningIconImageNode, this);
		
		iconDecoratorImageNode = new IconImageNode {
			NodeId = 4,
			Size = new Vector2(32.0f, 32.0f),
			Position = new Vector2(24.0f, 18.0f),
			IconId = 62145,
			WrapMode = 1,
			IsVisible = true,
		};
		System.NativeController.AttachNode(iconDecoratorImageNode, this);

		nameTextNode = new TextNode {
			NodeId = 5,
			Size = new Vector2(184.0f, 24.0f),
			Position = new Vector2(76.0f, 22.0f),
			FontType = FontType.Axis,
			FontSize = 14,
			LineSpacing = 14,
			AlignmentType = AlignmentType.Left,
			TextColor = new Vector4(232.0f, 255.0f, 254.0f, 255.0f) / 255.0f,
			TextFlags = TextFlags.Edge,
			TextFlags2 = TextFlags2.Ellipsis,
			Text = "Your Name Here",
		};

		if (PlayerState.Instance()->IsLevelSynced is not 0) {
			nameTextNode.Size -= new Vector2(18.0f, 0.0f);
			nameTextNode.Position += new Vector2(18.0f, 0.0f);
		}
		
		System.NativeController.AttachNode(nameTextNode, this);

		BuildTimelines();

		Timeline?.PlayAnimation(System.PartyListController.Config.Animation ? 1 : 2);
	}

	public WarningState? Warning {
		get;
		set {
			field = value;

			if (value is not null) {
				iconDecoratorImageNode.IconId = value.IconId;
				nameTextNode.Text = value.Message;
			}
		}
	}

	public SeString NameText {
		get => nameTextNode.Text;
		set => nameTextNode.Text = value;
	}

	public bool ShowName {
		get => nameTextNode.IsVisible;
		set => nameTextNode.IsVisible = value;
	}
	
	public bool ShowIcon {
		get => iconDecoratorImageNode.IsVisible;
		set => iconDecoratorImageNode.IsVisible = value;
	}

	public AddonPartyList.PartyListMemberStruct MemberStruct { get; set; }

	private void BuildTimelines() {
		AddTimeline(new TimelineBuilder()
			.BeginFrameSet(1, 120)
			.AddLabel(1, 1, AtkTimelineJumpBehavior.Start, 0) // Label 1: Pulsing Animation
			.AddLabel(60, 0, AtkTimelineJumpBehavior.LoopForever, 1)
			.AddLabel(61, 2, AtkTimelineJumpBehavior.Start, 0) // Label 2: No Animation
			.AddLabel(120, 0, AtkTimelineJumpBehavior.LoopForever, 2)
			.EndFrameSet()
			.Build());
		
		warningIconImageNode.AddTimeline(new TimelineBuilder()
			.BeginFrameSet(1, 60)
			.AddFrame(1, alpha: 0)
			.AddFrame(30, alpha: 255)
			.AddFrame(60, alpha: 0)
			.EndFrameSet()
			.BeginFrameSet(61, 120)
			.AddFrame(61, alpha: 255)
			.EndFrameSet()
			.Build());
		
		iconDecoratorImageNode.AddTimeline(new TimelineBuilder()
			.BeginFrameSet(1, 60)
			.AddFrame(15, alpha: 255)
			.AddFrame(30, alpha: 0)
			.AddFrame(45, alpha: 255)
			.EndFrameSet()
			.BeginFrameSet(61, 120)
			.AddFrame(61, alpha: 0)
			.EndFrameSet()
			.Build());
		
		nameTextNode.AddTimeline(new TimelineBuilder()
			.BeginFrameSet(1, 60)
			.AddFrame(1, textOutlineColor: defaultOutlineColor.AsVector3())
			.AddFrame(30, textOutlineColor: System.PartyListController.Config.OutlineColor.AsVector3())
			.AddFrame(60, textOutlineColor: defaultOutlineColor.AsVector3())
			.EndFrameSet()
			.BeginFrameSet(61, 120)
			.AddFrame(61, textOutlineColor: System.PartyListController.Config.OutlineColor.AsVector3())
			.EndFrameSet()
			.Build());
	}

	public void UpdateNameColor() {
		nameTextNode.Timeline?.UpdateKeyFrame(30, KeyFrameGroupType.TextEdge, textOutlineColor: System.PartyListController.Config.OutlineColor.AsVector3());
		nameTextNode.Timeline?.UpdateKeyFrame(61, KeyFrameGroupType.TextEdge, textOutlineColor: System.PartyListController.Config.OutlineColor.AsVector3());
	}

	public void Save()
		=> Save(PartyListNodePath);
	
	public void Load()
		=> Load(PartyListNodePath);
}