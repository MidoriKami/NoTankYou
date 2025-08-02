using System.Numerics;
using Dalamud.Utility;
using FFXIVClientStructs.FFXIV.Client.UI;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Classes;
using KamiToolKit.Classes.TimelineBuilding;
using KamiToolKit.Nodes;

namespace NoTankYou.Classes;

public unsafe class PartyListOverlay {
	public required WarningState Warning { get; init; }
	public required AddonPartyList.PartyListMemberStruct* MemberStruct { get; init; }

	private SimpleComponentNode? backgroundContainer;
	private SimpleComponentNode? foregroundContainer;
	private NineGridNode? glowNode;
	private IconImageNode? warningIconNode;
	
	public void Attach() {
		var componentNode = MemberStruct->PartyMemberComponent->OwnerNode;
		var collisionNode = (AtkCollisionNode*) MemberStruct->PartyMemberComponent->UldManager.SearchNodeById(23);
		
		var componentNodeSize = new Vector2(componentNode->Width, componentNode->Height);
		var componentNodePosition = new Vector2(componentNode->X, componentNode->Y);
		
		var collisionNodeSize = new Vector2(collisionNode->Width, collisionNode->Height);
		var collisionNodePosition = new Vector2(collisionNode->X, collisionNode->Y);

		backgroundContainer = new SimpleOverlayNode {
			Size = componentNodeSize,
			Position = componentNodePosition,
		};
		System.NativeController.AttachNode(backgroundContainer, componentNode, NodePosition.BeforeTarget);

		glowNode = new SimpleNineGridNode {
			Size = collisionNodeSize,
			Origin = collisionNodeSize / 2.0f,
			Position = collisionNodePosition,
			TexturePath = "ui/uld/PartyListTargetBase.tex",
			TextureSize = new Vector2(48.0f, 48.0f),
			TextureCoordinates = new Vector2(160.0f, 0.0f),
			Offsets = new Vector4(20),
			IsVisible = true,
			Color = Vector4.Zero,
			Alpha = 1.0f,
			AddColor = new Vector3(0.70f, 0.4f, 0.4f),
		};
		System.NativeController.AttachNode(glowNode, backgroundContainer);
		
		foregroundContainer = new SimpleOverlayNode {
			Size = componentNodeSize, 
			Position = componentNodePosition,
		};
		System.NativeController.AttachNode(foregroundContainer, componentNode, NodePosition.AfterTarget);

		warningIconNode = new IconImageNode {
			Size = new Vector2(16.0f, 16.0f),
			Origin = new Vector2(8.0f, 8.0f),
			Position = new Vector2(24.0f, 18.0f) + new Vector2(16.0f, 0.0f),
			TextureSize = new Vector2(28.0f, 28.0f),
			IsVisible = true,
			WrapMode = 2,
			ImageNodeFlags = 0,
		};
		System.NativeController.AttachNode(warningIconNode, foregroundContainer);

		BuildBackgroundAnimations();
		BuildForegroundAnimations();
		
		ApplyConfigurationOptions();
	}

	public void Detach() {
		System.NativeController.DetachNode(backgroundContainer, () => {
			backgroundContainer?.Dispose();
			backgroundContainer	= null;
		});
		
		System.NativeController.DetachNode(foregroundContainer, () => {
			foregroundContainer?.Dispose();
			foregroundContainer	= null;
		});
	}
	
	private void BuildBackgroundAnimations() {
		backgroundContainer?.AddTimeline(new TimelineBuilder()
			.BeginFrameSet(1, 120)
			.AddLabel(1, 1, AtkTimelineJumpBehavior.Start, 0)
			.AddLabel(60, 0, AtkTimelineJumpBehavior.LoopForever, 1)
			.AddLabel(61, 2, AtkTimelineJumpBehavior.Start, 0)
			.AddLabel(120, 0, AtkTimelineJumpBehavior.LoopForever, 2)
			.EndFrameSet()
			.Build());
		
		glowNode?.AddTimeline(new TimelineBuilder()
			.BeginFrameSet(1, 60)
			.AddFrame(1, scale: new Vector2(1.0f, 0.97f), alpha: 155)
			.AddFrame(30, scale: new Vector2(1.05f, 1.20f), alpha: 255)
			.AddFrame(60, scale: new Vector2(1.0f, 0.97f), alpha: 155)
			.EndFrameSet()
			.BeginFrameSet(61, 120)
			.AddFrame(61, scale: new Vector2(1.0f, 1.0f), alpha: 255)
			.EndFrameSet()
			.Build());
	}

	private void BuildForegroundAnimations() {
		foregroundContainer?.AddTimeline(new TimelineBuilder()
			.BeginFrameSet(1, 120)
			.AddLabel(1, 1, AtkTimelineJumpBehavior.Start, 0)
			.AddLabel(60, 0, AtkTimelineJumpBehavior.LoopForever, 1)
			.AddLabel(61, 2, AtkTimelineJumpBehavior.Start, 0)
			.AddLabel(120, 0, AtkTimelineJumpBehavior.LoopForever, 2)
			.EndFrameSet()
			.Build());
		
		warningIconNode?.AddTimeline(new TimelineBuilder()
			.BeginFrameSet(1, 60)
			.AddFrame(1, scale: new Vector2(1.0f, 1.0f), alpha: 155)
			.AddFrame(30, scale: new Vector2(1.55f, 1.55f), alpha: 255)
			.AddFrame(60, scale: new Vector2(1.0f, 1.0f), alpha: 155)
			.EndFrameSet()
			.BeginFrameSet(61, 120)
			.AddFrame(61, scale: new Vector2(1.55f, 1.55f), alpha: 255)
			.EndFrameSet()
			.Build());
	}

	public void ApplyConfigurationOptions() {
		if (warningIconNode is not null) {
			if (System.PartyListController.Config.UseModuleIcons) {
				if (Warning.SourceModule.GetAttribute<ModuleIconAttribute>() is { } iconInfo) {
					warningIconNode.IconId = iconInfo.ModuleIcon;
				}
				
				warningIconNode.EnableEventFlags = true;
				warningIconNode.Tooltip = Warning.Message;
			}
			else {
				warningIconNode.IconId = 60074;
			}
		}

		if (backgroundContainer is not null) {
			backgroundContainer.IsVisible = System.PartyListController.Config.ShowGlow;
		}

		if (foregroundContainer is not null) {
			foregroundContainer.IsVisible = System.PartyListController.Config.ShowIcon;
		}

		if (glowNode is not null) {
			var color = System.PartyListController.Config.GlowColor;
			
			glowNode.AddColor = color.AsVector3();
			glowNode.Alpha = color.W;
		}
		
		backgroundContainer?.Timeline?.PlayAnimation(System.PartyListController.Config.Animation ? 1 : 2);
		foregroundContainer?.Timeline?.PlayAnimation(System.PartyListController.Config.Animation ? 1 : 2);
	}
}