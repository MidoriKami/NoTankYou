using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Numerics;
using Dalamud.Utility.Numerics;
using FFXIVClientStructs.FFXIV.Client.UI;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using FFXIVClientStructs.FFXIV.Component.GUI;
using ImGuiNET;
using ImGuiScene;
using KamiLib.Utilities;
using NoTankYou.DataModels;
using NoTankYou.Models;
using NoTankYou.System;
using NoTankYou.Utilities;
using PartyListMemberStruct = FFXIVClientStructs.FFXIV.Client.UI.AddonPartyList.PartyListMemberStruct;

namespace NoTankYou.Views.Components;
// Change this to be per party member, and pass in MaxBy which can be null to draw, if null, and state was dirty then reset else leave alone
public unsafe class PartyMemberOverlay
{
    private static TextureWrap? WarningIcon => ImageCache.Instance.GetImage("Warning.png");
    private static ImDrawListPtr DrawList => ImGui.GetBackgroundDrawList();
    private static AddonPartyList* Addon => (AddonPartyList*) Service.GameGui.GetAddonByName("_PartyList");
    private static AgentHUD* Agent => AgentModule.Instance()->GetAgentHUD();
    private static readonly Stopwatch AnimationStopwatch = Stopwatch.StartNew();
    private static PartyListConfig Config => NoTankYouSystem.PartyListController.Config;
    private static bool AnimationState => !Config.Animation || AnimationStopwatch.ElapsedMilliseconds < Config.AnimationPeriod / 2.0f;

    public static void Update()
    {
        if(AnimationStopwatch.ElapsedMilliseconds > Config.AnimationPeriod) AnimationStopwatch.Restart();
    }
    
    public static void Draw(WarningState? warningState)
    {
        if (!IsAddonReady(&Addon->AtkUnitBase)) return;
        if (!Agent->AgentInterface.IsAgentActive()) return;
        if (warningState is null) return;

        if (GetPartyListMemberForObjectId(warningState.SourceObjectId) is not { } partyMember) return;
        
         if (Config.JobIcon) DrawWarningShield(partyMember);
         if (Config.PlayerName) DrawPlayerName(partyMember);
         if (Config.WarningText) DrawWarningText(partyMember, warningState);
    }

    public static void Reset(PartyListMemberStruct member, PartyListMemberStruct chocobo)
    {
        member.Name->AtkResNode.ToggleVisibility(chocobo.Name->AtkResNode.IsVisible);
        member.Name->EdgeColor = chocobo.Name->EdgeColor;
        member.ClassJobIcon->AtkResNode.ToggleVisibility(chocobo.ClassJobIcon->AtkResNode.IsVisible);
    }

    private static void DrawWarningShield(PartyListMemberStruct member)
    {
        if (WarningIcon is null) return;
        
        var jobIconPosition = GetScreenPosition((AtkResNode*) member.ClassJobIcon);
        var warningShieldSize = new Vector2(WarningIcon.Width, WarningIcon.Height) * Addon->AtkUnitBase.Scale;

        if (AnimationState)
        {
            member.ClassJobIcon->AtkResNode.ToggleVisibility(false);
            DrawList.AddImage(WarningIcon.ImGuiHandle, jobIconPosition, jobIconPosition + warningShieldSize);
        }
        else
        {
            member.ClassJobIcon->AtkResNode.ToggleVisibility(true);
        }
    }

    private static void DrawPlayerName(PartyListMemberStruct member)
    {
        if (AnimationState)
        {
            member.Name->EdgeColor = Config.OutlineColor.ToByteColor();
        }
        else
        {
            member.Name->EdgeColor = Addon->Chocobo.Name->EdgeColor;
        }
    }

    private static void DrawWarningText(PartyListMemberStruct member, WarningState warning)
    {
        var position = GetScreenPosition((AtkResNode*) member.Name);
        position += new Vector2(member.Name->AtkResNode.Width * Addon->AtkUnitBase.Scale, 7.5f);
        position -= ImGui.CalcTextSize(warning.Message);

        DrawList.AddText(position, AnimationState ? ImGui.GetColorU32(Config.TextColor) : ImGui.GetColorU32(KnownColor.White.AsVector4()), warning.Message);
    }
    
    private static bool IsAddonReady(AtkUnitBase* addon)
    {
        if (addon is null) return false;
        if (addon->RootNode is null) return false;
        if (addon->RootNode->ChildNode is null) return false;

        return true;
    }

    private static PartyListMemberStruct? GetPartyListMemberForObjectId(ulong objectId)
    {
        for (var i = 0; i < Agent->PartyMemberCount; ++i)
        {
            if (Agent->PartyMemberListSpan[i].ObjectId == objectId)
            {
                return Addon->PartyMember[i];
            }
        }

        return null;
    }

    private static Vector2 GetScreenPosition(AtkResNode* element)
    {
        AtkResNode* previousNode = null;
        AtkResNode* currentNode = element;

        var positionStack = new Stack<Vector2>();

        while (currentNode is not null)
        {
            positionStack.Push(new Vector2(currentNode->X, currentNode->Y));
            
            previousNode = currentNode;
            currentNode = currentNode->ParentNode;
        }

        var basePosition = positionStack.Pop();
        var subPositions = positionStack.Aggregate((a, b) => a + b);

        return basePosition + new Vector2(subPositions.X * previousNode->ScaleX, subPositions.Y * previousNode->ScaleY);
    }
}