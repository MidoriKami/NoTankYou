using System.Diagnostics;
using System.Linq;
using System.Numerics;
using Dalamud.Interface.Windowing;
using Dalamud.Utility.Signatures;
using ImGuiNET;
using ImGuiScene;
using NoTankYou.Configuration.Components;
using NoTankYou.Configuration.Overlays;
using NoTankYou.System;
using NoTankYou.Utilities;

namespace NoTankYou.UserInterface.Windows;

internal class PartyListOverlayWindow : Window
{
    private readonly Stopwatch AnimationStopwatch = new();
    private PartyOverlaySettings Settings => Service.ConfigurationManager.CharacterConfiguration.PartyOverlay;

    private readonly TextureWrap WarningIcon;
    private Vector2 Scale { get; set; }

    private delegate bool IsInSanctuary();

    [Signature("E8 ?? ?? ?? ?? 84 C0 75 21 48 8B 4F 10")]
    private readonly IsInSanctuary SanctuaryFunction = null!;

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
    }

    public override void PreOpenCheck()
    {
        if (Condition.ShouldShowWarnings()) IsOpen = true;
        if (!Condition.ShouldShowWarnings()) IsOpen = false;
        if (Settings.DisableInSanctuary.Value && SanctuaryFunction()) IsOpen = false;

        if (IsOpen == false) ResetAllAnimation();
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
        foreach (var player in Service.PartyListAddon)
        {
            if (player.IsTargetable())
            {
                if (player.PlayerCharacter is { } playerCharacter)
                {
                    // Get all Logic Modules for this classjob
                    var modules = Service.ModuleManager.GetModulesForClassJob(playerCharacter.ClassJob.Id);

                    // Filter to only modules that are enabled for PartyFrame Overlay
                    var enabledModules = modules
                        .Where(module => module.ParentModule.GenericSettings.PartyFrameOverlay.Value);

                    // Get Highest Warning for remaining modules
                    var highestWarning = enabledModules
                        .Select(module => module.ShouldShowWarning(playerCharacter))
                        .OfType<WarningState>()
                        .DefaultIfEmpty(null)
                        .Aggregate((i1, i2) => i1!.Priority > i2!.Priority ? i1 : i2);

                    // If the warning exists
                    if (highestWarning is not null)
                    {
                        if(Settings.JobIcon.Value)
                            AnimateJobIcon(player);

                        if(Settings.PlayerName.Value)
                            AnimatePlayerName(player);

                        if(Settings.WarningText.Value)
                            AnimateWarningText(player, highestWarning.MessageLong);
                    }
                    else
                    {
                        ResetAnimation(player);
                    }
                }
            }
            else
            {
                ResetAnimation(player);
            }
        }

        if (AnimationStopwatch.ElapsedMilliseconds >= 1300)
            AnimationStopwatch.Restart();
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
        player.UserInterface.SetIconVisibility(player.AgentData.ValidData);
        player.UserInterface.SetPlayerNameOutlineColor(Vector4.Zero);
    }
}