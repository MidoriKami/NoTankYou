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
using KamiLib.Atk;
using KamiLib.Utilities;
using NoTankYou.DataModels;
using NoTankYou.Models;
using NoTankYou.Utilities;
using PartyListMemberStruct = FFXIVClientStructs.FFXIV.Client.UI.AddonPartyList.PartyListMemberStruct;

namespace NoTankYou.Views.Components;

public unsafe class PartyMemberOverlay
{
    private static TextureWrap? WarningIcon => ImageCache.Instance.GetImage("Warning.png");
    private static ImDrawListPtr DrawList => ImGui.GetBackgroundDrawList();
    private static AddonPartyList* Addon => (AddonPartyList*) Service.GameGui.GetAddonByName("_PartyList");
    private static AgentHUD* Agent => AgentModule.Instance()->GetAgentHUD();

    private HudPartyMember HudData => Agent->PartyMemberListSpan[memberIndex];
    private PartyListMemberStruct AddonData => Addon->PartyMember[memberIndex];
    
    private readonly Stopwatch animationStopwatch = Stopwatch.StartNew();
    private readonly PartyListConfig config;
    private readonly int memberIndex;
    private bool dirty;

    public uint ObjectId => HudData.ObjectId;
    private bool AnimationState => !config.Animation || animationStopwatch.ElapsedMilliseconds < config.AnimationPeriod / 2.0f;
    
    public PartyMemberOverlay( PartyListConfig configuration, int memberId)
    {
        config = configuration;
        memberIndex = memberId;
    }

    public void Update()
    {
        if(animationStopwatch.ElapsedMilliseconds > config.AnimationPeriod) animationStopwatch.Restart();
    }
    
    public void DrawWarning(WarningState? warning)
    {
        if (warning is null)
        {
            Reset();
            return;
        }

        if (config.JobIcon) AnimateWarningShield();
        if (config.PlayerName) AnimatePlayerName();
        if (config.WarningText) AnimateWarningText(warning);
    }

    private void AnimateWarningShield()
    {
        if (AnimationState)
        {
            HideJobIcon();
            DrawWarningShield();
        }
        else
        {
            ShowJobIcon();
        }
    }

    private void AnimatePlayerName()
    {
        if (AnimationState)
        {
            ColorPlayerName();
        }
        else
        {
            ResetColorPlayerName();
        }
    }

    private void AnimateWarningText(WarningState warning)
    {
        DrawWarningText(warning, AnimationState ? config.TextColor : KnownColor.White.AsVector4());
    }

    private void DrawWarningShield()
    {
        if (WarningIcon is null) return;
        
        var jobIconPosition = GetScreenPosition((AtkResNode*) AddonData.ClassJobIcon) + ImGui.GetMainViewport().Pos;
        var warningShieldSize = new Vector2(WarningIcon.Width, WarningIcon.Height) * Addon->AtkUnitBase.Scale;
        
        DrawList.AddImage(WarningIcon.ImGuiHandle, jobIconPosition, jobIconPosition + warningShieldSize);
    }
    
    private void DrawWarningText(WarningState warning, Vector4 color)
    {
        var position = GetScreenPosition((AtkResNode*) AddonData.Name) + ImGui.GetMainViewport().Pos;
        position += new Vector2(AddonData.Name->AtkResNode.Width * Addon->AtkUnitBase.Scale, 7.5f);
        position -= ImGui.CalcTextSize(warning.Message);

        DrawList.AddText(position, ImGui.GetColorU32(color), warning.Message);
    }
    
    private void HideJobIcon()
    {
        AddonData.ClassJobIcon->AtkResNode.ToggleVisibility(false);
        dirty = true;
    }

    private void ShowJobIcon()
    {
        AddonData.ClassJobIcon->AtkResNode.ToggleVisibility(true);
        dirty = true;
    }

    private void ColorPlayerName()
    {
        AddonData.Name->EdgeColor = config.OutlineColor.ToByteColor();
        dirty = true;
    }

    private void ResetColorPlayerName()
    {
        AddonData.Name->EdgeColor = Addon->Chocobo.Name->EdgeColor;
    }

    public void Reset(bool force = false)
    {
        if (!Node.IsAddonReady((AtkUnitBase*) Addon)) return;
        
        if (dirty || force)
        {
            AddonData.Name->EdgeColor = Addon->Chocobo.Name->EdgeColor;
            AddonData.ClassJobIcon->AtkResNode.ToggleVisibility(true);
            dirty = false;
        }
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