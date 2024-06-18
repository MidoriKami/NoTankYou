using System;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using Dalamud.Utility;
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
	private readonly ImageNode[] warningIndicators;

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
		
		// Get all modules, minus test module.
		var moduleNames = Enum.GetValues<ModuleName>()[..^1];
		
		warningIndicators = new ImageNode[moduleNames.Length];

		foreach (var moduleType in moduleNames) {
			var warningTypeNode = new ImageNode {
				NodeID = 20000 + (uint)moduleType,
				NodeFlags = NodeFlags.Visible,
				Size = new Vector2(24.0f, 24.0f),
				X = jobIconNode->GetX() + 16.0f,
				Y = jobIconNode->GetY() + 16.0f,
				IsVisible = false,
			};
			
			warningIndicators[(int) moduleType] = warningTypeNode;

			if (moduleType is ModuleName.Tanks) {
				warningTypeNode.LoadTexture("ui/uld/fourth/LFG_hr1.tex");
				warningTypeNode.TextureCoordinates = new Vector2(0.0f, 216.0f);
				warningTypeNode.ImageNodeFlags = 0;
				warningTypeNode.Size = new Vector2(56.0f, 56.0f);
				warningTypeNode.Scale = new Vector2(24.0f / 56.0f);
			}
			else {
				warningTypeNode.LoadIcon(moduleType.GetAttribute<ModuleIconAttribute>()!.SimpleIcon);
			}

			warningTypeNode.AttachNode((AtkResNode*)jobIconNode, NodePosition.AfterTarget);
		}
		
		imageNode = new ImageNode {
			NodeID = 10000 + containerNode->NodeId,
			NodeFlags = NodeFlags.Visible,
			Size = new Vector2(32.0f, 32.0f),
			X = jobIconNode->GetX(),
			Y = jobIconNode->GetY(),
			IsVisible = false,
		};

		imageNode.LoadIcon(60074);
		imageNode.AttachNode((AtkResNode*)jobIconNode, NodePosition.AfterTarget);
		
		memberUiData->PartyMemberComponent->UldManager.UpdateDrawNodeList();
	}

	public void Dispose() {
		imageNode.Dispose();

		foreach (var indicator in warningIndicators) {
			indicator.Dispose();
		}
		
		Service.Framework.RunOnFrameworkThread(() => {
			partyComponent->UldManager.UpdateDrawNodeList();
		});
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
			AnimateWarningIcon(warning);
		}

		if (System.PartyListController.Config.PlayerName) {
			AnimePlayerNameColor();
		}
	}

	private void AnimateWarningIcon(WarningState warning) {
		if (AnimationState) {
			imageNode.IsVisible = true;
			jobIconNode->ToggleVisibility(false);

			foreach (var index in Enumerable.Range(0, Enum.GetValues<ModuleName>().Length - 1)) {
				warningIndicators[index].IsVisible = index == (int) warning.SourceModule;
			}
		}
		else {
			imageNode.IsVisible = false;
			jobIconNode->ToggleVisibility(true);
			foreach (var index in Enumerable.Range(0, Enum.GetValues<ModuleName>().Length - 1)) {
				warningIndicators[index].IsVisible = false;
			}
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
		jobIconNode->ToggleVisibility(true);

		imageNode.IsVisible = false;
		
		foreach (var index in Enumerable.Range(0, Enum.GetValues<ModuleName>().Length - 1)) {
			warningIndicators[index].IsVisible = false;
		}
	}
}