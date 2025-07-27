using System.Collections.Generic;
using System.Numerics;
using Dalamud.Utility;
using FFXIVClientStructs.FFXIV.Client.Game.UI;
using FFXIVClientStructs.FFXIV.Client.UI;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiLib.Configuration;
using KamiToolKit.Classes.TimelineBuilding;
using KamiToolKit.Nodes;
using KamiToolKit.Extensions;

namespace NoTankYou.Classes;

public unsafe class PartyListOverlayNode : SimpleOverlayNode {

	private readonly IconImageNode jobIconNode;
	private readonly IconImageNode warningIconNode;
	private readonly TextNode nameTextNode;
	private readonly TextNode warningTextNode;

	private Vector4 defaultOutlineColor = new Vector4(8.0f, 80.0f, 152.0f, 255.0f) / 255.0f;
	
    private static string PartyListNodePath => Service.PluginInterface.GetCharacterFileInfo(Service.ClientState.LocalContentId, "PartyListNode.style.json").FullName;

	public PartyListOverlayNode() {
		jobIconNode = new IconImageNode {
			NodeId = 2,
			Size = new Vector2(32.0f, 32.0f),
			Position = new Vector2(24.0f, 18.0f),
			WrapMode = 1,
			IsVisible = true,
		};
		System.NativeController.AttachNode(jobIconNode, this);
		
		warningIconNode = new IconImageNode {
			NodeId = 3,
			Size = new Vector2(32.0f, 32.0f),
			Origin = new Vector2(16.0f, 16.0f),
			Position = new Vector2(24.0f, 18.0f),
			IconId = 60074,
			WrapMode = 1,
			IsVisible = true,
		};
		System.NativeController.AttachNode(warningIconNode, this);

		nameTextNode = new TextNode {
			NodeId = 4,
			Size = new Vector2(184.0f, 24.0f),
			Position = new Vector2(76.0f, 22.0f),
			FontType = FontType.Axis,
			FontSize = 14,
			LineSpacing = 14,
			TextOutlineColor = System.PartyListController.Config.OutlineColor,
			AlignmentType = AlignmentType.Left,
			TextColor = new Vector4(232.0f, 255.0f, 254.0f, 255.0f) / 255.0f,
			TextFlags = TextFlags.Edge,
			TextFlags2 = TextFlags2.Ellipsis,
			Text = "Your Name Here",
		};
		System.NativeController.AttachNode(nameTextNode, this);

		warningTextNode = new TextNode {
			NodeId = 5,
			Size = new Vector2(184.0f, 24.0f),
			Position = new Vector2(76.0f, 22.0f),
			FontType = FontType.Axis,
			FontSize = 14,
			LineSpacing = 14,
			AlignmentType = AlignmentType.Left,
			TextOutlineColor = System.PartyListController.Config.OutlineColor,
			TextColor = new Vector4(232.0f, 255.0f, 254.0f, 255.0f) / 255.0f,
			TextFlags = TextFlags.Edge,
			TextFlags2 = TextFlags2.Ellipsis,
			Text = "Your Warning Here",
		};

		if (PlayerState.Instance()->IsLevelSynced is not 0) {
			warningTextNode.Size -= new Vector2(18.0f, 0.0f);
			warningTextNode.Position += new Vector2(18.0f, 0.0f);
		}
		System.NativeController.AttachNode(warningTextNode, this);

		BuildTimelines();

		Timeline?.PlayAnimation(System.PartyListController.Config.Animation ? 1 : 2);
	}

	public override List<string> OnLoadOmittedProperties => [ "Position" ];

	public WarningState? Warning {
		get;
		set {
			field = value;

			if (value is not null && MemberStruct is not null) {
				warningTextNode.Text = value.Message;
				jobIconNode.IconId = MemberStruct->ClassJobIcon->GetIconId();
				nameTextNode.Text = MemberStruct->Name->GetText().ToString();

				if (System.PartyListController.Config.UseModuleIcons) {
					if (value.SourceModule.GetAttribute<ModuleIconAttribute>() is { } iconInfo) {
						warningIconNode.IconId = iconInfo.ModuleIcon;
					}
				}
			}
		}
	}

	public void UpdateNameString() {
		if (MemberStruct is null) return;
		if (nameTextNode.Text.ToString() != MemberStruct->Name->ToString()) {
			nameTextNode.Text = MemberStruct->Name->GetText().ToString();
		}
	}

	public AddonPartyList.PartyListMemberStruct* MemberStruct { get; set; }

	private void BuildTimelines() {
		AddTimeline(new TimelineBuilder()
			.BeginFrameSet(1, 120)
			.AddLabel(1, 1, AtkTimelineJumpBehavior.Start, 0) // Label 1: Pulsing Animation
			.AddLabel(60, 0, AtkTimelineJumpBehavior.LoopForever, 1)
			.AddLabel(61, 2, AtkTimelineJumpBehavior.Start, 0) // Label 2: No Animation
			.AddLabel(120, 0, AtkTimelineJumpBehavior.LoopForever, 2)
			.EndFrameSet()
			.Build());
		
		warningIconNode.AddTimeline(new TimelineBuilder()
			.BeginFrameSet(1, 60)
			.AddFrame(1, scale: new Vector2(0.75f, 0.75f), alpha: 0, position: new Vector2(24.0f, 18.0f))
			.AddFrame(20, scale: new Vector2(0.75f, 0.75f), alpha: 0)
			.AddFrame(30, scale: new Vector2(1.4f, 1.4f), alpha: 255)
			.AddFrame(55, scale: new Vector2(1.4f, 1.4f), alpha: 255)
			.AddFrame(60, scale: new Vector2(0.75f, 0.75f), alpha: 0)
			.EndFrameSet()
			.BeginFrameSet(61, 120)
			.AddFrame(61, scale: new Vector2(0.66f, 0.66f), alpha: 255, position: new Vector2(24.0f, 18.0f) + new Vector2(8.0f, 8.0f))
			.EndFrameSet()
			.Build());
		
		nameTextNode.AddTimeline(new TimelineBuilder()
			.BeginFrameSet(1, 60)
			.AddFrame(1, alpha: 255)
			.AddFrame(20, alpha: 255)
			.AddFrame(30, alpha: 0)
			.AddFrame(55, alpha: 0)
			.AddFrame(60, alpha: 255)
			.EndFrameSet()
			.BeginFrameSet(61, 120)
			.AddFrame(61, alpha: 255)
			.EndFrameSet()
			.Build());
		
		warningTextNode.AddTimeline(new TimelineBuilder()
			.BeginFrameSet(1, 60)
			.AddFrame(1, alpha: 0)
			.AddFrame(20, alpha: 0)
			.AddFrame(30, alpha: 255)
			.AddFrame(55, alpha: 255)
			.AddFrame(60, alpha: 0)
			.EndFrameSet()
			.BeginFrameSet(61, 120)
			.AddFrame(61, alpha: 0)
			.EndFrameSet()
			.Build());
	}

	public void UpdateNameColor() {
		nameTextNode.TextOutlineColor = System.PartyListController.Config.OutlineColor;
		warningTextNode.TextOutlineColor = System.PartyListController.Config.OutlineColor;
	}

	public void Save()
		=> Save(PartyListNodePath);
	
	public void Load()
		=> Load(PartyListNodePath);
}