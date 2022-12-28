using System.Diagnostics;
using System.Linq;
using System.Numerics;
using Dalamud.Game.ClientState.Conditions;
using Dalamud.Interface.Windowing;
using Dalamud.Utility.Signatures;
using ImGuiNET;
using ImGuiScene;
using KamiLib.Utilities;
using Lumina.Excel.GeneratedSheets;
using NoTankYou.Configuration.Components;
using NoTankYou.System;
using NoTankYou.Utilities;
using Condition = NoTankYou.Utilities.Condition;

namespace NoTankYou.UserInterface.Windows;

internal class PartyListOverlayWindow : Window
{
    public static Stopwatch AnimationStopwatch { get; } = new();
    private PartyOverlaySettings Settings => Service.ConfigurationManager.CharacterConfiguration.PartyOverlay;

    private readonly TextureWrap WarningIcon;
    private Vector2 Scale { get; set; }

    private readonly WarningState DemoWarning;

    private bool InSanctuaryArea;

    public PartyListOverlayWindow() : base($"###PartyListOverlay")
    {
        SignatureHelper.Initialise(this);

        WarningIcon = Image.LoadImage("Warning");

        Flags |= ImGuiWindowFlags.NoDecoration;
        Flags |= ImGuiWindowFlags.NoBackground;
        Flags |= ImGuiWindowFlags.NoInputs;
        Flags |= ImGuiWindowFlags.NoBringToFrontOnFocus;
        Flags |= ImGuiWindowFlags.NoFocusOnAppearing;
        Flags |= ImGuiWindowFlags.NoNavFocus;

        ForceMainWindow = true;

        AnimationStopwatch.Start();

        var demoAction = Service.DataManager.GetExcelSheet<Action>()!.GetRow(67)!;

        DemoWarning = new WarningState
        {
            MessageShort = "Sample Warning",
            IconID = demoAction.Icon,
            IconLabel = "Sample",
            Priority = 11,
            MessageLong = "NoTankYou Sample Warning"
        };
    }

    public bool ShouldOpenWindow()
    {
        if (!PartyListAddon.DataAvailable) return false;

        if (!Condition.ShouldShowWarnings()) return false;

        if (Settings.PreviewMode.Value) return true;

        return true;
    }

    public override void PreOpenCheck()
    {
        IsOpen = ShouldOpenWindow();

        if (!IsOpen) ResetAllAnimation();
    }

    public override void Update()
    {
        InSanctuaryArea = FFXIVClientStructs.FFXIV.Client.Game.GameMain.IsInSanctuary();
    }

    public override void PreDraw()
    {
        var positionInfo = PartyListAddon.GetPositionInfo();

        Scale = positionInfo.Scale;
        Position = positionInfo.Position;
        Size = positionInfo.Size;
    }

    public override void Draw()
    {
        if (Settings.PreviewMode.Value)
        {
            foreach (var player in Service.PartyListAddon)
            {
                DisplayWarning(DemoWarning, player);
            }
        }
        else
        {
            foreach (var player in Service.PartyListAddon)
            {
                if (player.IsTargetable() && player.PlayerCharacter is { } playerCharacter)
                {
                    // If Solo Mode is Enabled and this isn't us.
                    if (Settings.SoloMode.Value && Service.ClientState.LocalPlayer?.ObjectId != playerCharacter.ObjectId) continue;

                    // Get all Logic Modules for this classjob
                    var modules = Service.ModuleManager.GetModulesForClassJob(playerCharacter.ClassJob.Id);

                    // Filter to only modules that are enabled for PartyFrame Overlay
                    var enabledModules = modules
                        .Where(module => module.ParentModule.GenericSettings.PartyFrameOverlay.Value)
                        .Where(module => !(module.ParentModule.GenericSettings.DisableInSanctuary.Value && InSanctuaryArea) );

                    // Get Highest Warning for remaining modules
                    var highestWarning = enabledModules
                        .Select(module => module.ShouldShowWarning(playerCharacter))
                        .OfType<WarningState>()
                        .DefaultIfEmpty(null)
                        .Aggregate((i1, i2) => i1!.Priority > i2!.Priority ? i1 : i2);

                    // If the warning exists
                    if (highestWarning is not null)
                    {
                        DisplayWarning(highestWarning, player);
                    }
                    else
                    {
                        ResetAnimation(player);
                    }
                }
                else
                {
                    ResetAnimation(player);
                }
            }
        }

        if (AnimationStopwatch.ElapsedMilliseconds >= 1300)
            AnimationStopwatch.Restart();
    }

    private void DisplayWarning(WarningState warning, PartyListAddonData player)
    {
        if(Settings.JobIcon.Value || Settings.PreviewMode.Value)
            AnimateJobIcon(player);

        if(Settings.PlayerName.Value || Settings.PreviewMode.Value)
            AnimatePlayerName(player);

        if(Settings.WarningText.Value || Settings.PreviewMode.Value)
            AnimateWarningText(player, warning.MessageLong);
    }

    private void AnimateWarningText(PartyListAddonData partyMember, string warningText)
    {
        if (!Settings.FlashingEffects.Value)
        {
            DrawText(partyMember, warningText, Settings.WarningTextColor.Value);
        }
        else
        {
            if (AnimationStopwatch.ElapsedMilliseconds < 500)
            {
                DrawText(partyMember, warningText, Colors.White);

            }
            else if (AnimationStopwatch.ElapsedMilliseconds > 500)
            {
                DrawText(partyMember, warningText, Settings.WarningTextColor.Value);
            }
        }
    }

    private void DrawText(PartyListAddonData partyMember, string warningText, Vector4 color)
    {
        var namePosition = partyMember.UserInterface.GetNamePosition() * Scale;
        var nameSize = partyMember.UserInterface.GetNameSize() * Scale;

        var textSize = ImGui.CalcTextSize(warningText);
        var warningTextPosition = new Vector2(namePosition.X + nameSize.X - textSize.X, namePosition.Y - 10.0f);

        ImGui.SetCursorPos(warningTextPosition);
        ImGui.TextColored(color, warningText);
    }

    private void AnimateJobIcon(PartyListAddonData partyMember)
    {
        if (!Settings.FlashingEffects.Value)
        {
            partyMember.UserInterface.SetIconVisibility(false);
            DrawWarningShield(partyMember);
        }
        else
        {
            if (AnimationStopwatch.ElapsedMilliseconds < 500)
            {
                partyMember.UserInterface.SetIconVisibility(true);
            }
            else if (AnimationStopwatch.ElapsedMilliseconds > 500)
            {
                partyMember.UserInterface.SetIconVisibility(false);
                DrawWarningShield(partyMember);
            }
        }
    }

    private void AnimatePlayerName(PartyListAddonData partyMember)
    {
        if (!Settings.FlashingEffects.Value)
        {
            partyMember.UserInterface.SetPlayerNameOutlineColor(Settings.WarningOutlineColor.Value);
        }
        else
        {
            if (AnimationStopwatch.ElapsedMilliseconds < 500)
            {
                partyMember.UserInterface.SetPlayerNameOutlineColor(Vector4.Zero);

            }
            else if (AnimationStopwatch.ElapsedMilliseconds > 500)
            {
                partyMember.UserInterface.SetPlayerNameOutlineColor(Settings.WarningOutlineColor.Value);
            }
        }
    }

    private void DrawWarningShield(PartyListAddonData hudPartyIndex)
    {
        var iconPosition = hudPartyIndex.UserInterface.GetIconPosition();
        var iconSize = hudPartyIndex.UserInterface.GetIconSize();

        ImGui.SetCursorPos(iconPosition * Scale);
        ImGui.Image(WarningIcon.ImGuiHandle, iconSize * Scale);
    }

    private void ResetAllAnimation()
    {
        foreach (var player in Service.PartyListAddon)
        {
            ResetAnimation(player);
        }
    }

    private void ResetAnimation(PartyListAddonData player)
    {
        player.UserInterface.SetIconVisibility(Service.Condition[ConditionFlag.ParticipatingInCrossWorldPartyOrAlliance] || player.AgentData.ValidData);
        player.UserInterface.SetPlayerNameOutlineColor(Vector4.Zero);
    }
}