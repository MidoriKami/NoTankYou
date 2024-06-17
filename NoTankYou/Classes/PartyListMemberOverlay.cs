using System;
using System.Diagnostics;
using System.Numerics;
using Dalamud.Utility.Numerics;
using FFXIVClientStructs.FFXIV.Client.Graphics;
using FFXIVClientStructs.FFXIV.Client.UI;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Classes;
using KamiToolKit.Nodes;

namespace NoTankYou.Classes;

public unsafe class PartyListMemberOverlay : IDisposable {
	private static readonly Stopwatch AnimationStopwatch = Stopwatch.StartNew();

	private readonly ImageNode imageNode;

	private readonly AtkImageNode* jobIconNode;
	private readonly AtkTextNode* playerNameNode;
	private readonly AtkComponentBase* partyComponent;
	private ByteColor? originalOutlineColor;
	
	private static bool AnimationState => !System.PartyListController.Config.Animation || AnimationStopwatch.ElapsedMilliseconds < System.PartyListController.Config.AnimationPeriod / 2.0f;
	public PartyListMemberOverlay(AddonPartyList.PartyListMemberStruct* memberUiData) {
		var containerNode = memberUiData->PartyMemberComponent->OwnerNode;

		partyComponent = memberUiData->PartyMemberComponent;
		jobIconNode = memberUiData->ClassJobIcon;
		playerNameNode = memberUiData->Name;
		
		imageNode = new ImageNode {
			NodeID = 10000 + containerNode->NodeId,
			NodeFlags = NodeFlags.Visible,
			Size = new Vector2(32.0f, 32.0f),
			X = memberUiData->ClassJobIcon->GetX(),
			Y = memberUiData->ClassJobIcon->GetY(),
			IsVisible = false,
		};

		imageNode.LoadIcon(60074);
		imageNode.AttachNode((AtkResNode*)memberUiData->ClassJobIcon, NodePosition.AfterTarget);
		
		memberUiData->PartyMemberComponent->UldManager.UpdateDrawNodeList();
	}

	public void Dispose() {
		imageNode.Dispose();
		partyComponent->UldManager.UpdateDrawNodeList();
	}

	public void Update() {
		if (AnimationStopwatch.ElapsedMilliseconds > System.PartyListController.Config.AnimationPeriod) {
			AnimationStopwatch.Restart();
		}
	}

	public void DrawWarning(WarningState? warning) {
		if (warning is null) return;
		
		if (System.PartyListController.Config.JobIcon) {
			imageNode.Tooltip = warning.Message;
			AnimateWarningIcon();
		}

		if (System.PartyListController.Config.PlayerName) {
			AnimePlayerNameColor();
		}
	}

	private void AnimateWarningIcon() {
		if (AnimationState) {
			imageNode.IsVisible = true;
			jobIconNode->ToggleVisibility(false);
		}
		else {
			imageNode.IsVisible = false;
			jobIconNode->ToggleVisibility(true);
		}
	}

	private void AnimePlayerNameColor() {
		originalOutlineColor ??= playerNameNode->EdgeColor;
		if (originalOutlineColor is null) return;
		
		if (AnimationState) {
			playerNameNode->EdgeColor = System.PartyListController.Config.OutlineColor.ToByteColor();
		}
		else {
			playerNameNode->EdgeColor = originalOutlineColor.Value;
		}
	}

	public void EnableTooltip(AddonPartyList* addon) {
		imageNode.EnableTooltip(Service.AddonEventManager, addon);
	}
	
	public void Reset() {
		if (originalOutlineColor is null) return;
		
		playerNameNode->EdgeColor = originalOutlineColor.Value;
		playerNameNode->ToggleVisibility(true);
		jobIconNode->ToggleVisibility(true);

		imageNode.IsVisible = false;
	}
}