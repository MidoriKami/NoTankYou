using System.Diagnostics;
using System.Linq;
using System.Numerics;
using Dalamud.Interface.Windowing;
using ImGuiNET;
using ImGuiScene;
using KamiLib.Caching;
using KamiLib.Drawing;
using Lumina.Excel.GeneratedSheets;
using NoTankYou.DataModels;
using NoTankYou.System;
using NoTankYou.UserInterface.Windows;
using NoTankYou.Utilities;
using Condition = KamiLib.GameState.Condition;

namespace NoTankYou.UserInterface.OverlayWindows;

public class PartyListOverlayWindow : Window
{
    public static Stopwatch AnimationStopwatch { get; } = new();
    private static PartyOverlaySettings Settings => Service.ConfigurationManager.CharacterConfiguration.PartyOverlay;

    private readonly TextureWrap WarningIcon;
    private Vector2 Scale { get; set; }

    private readonly WarningState DemoWarning;

    private bool InSanctuaryArea;

    public PartyListOverlayWindow() : base($"###PartyListOverlay")
    {
        WarningIcon = Image.LoadImage("Warning");

        Flags |= ImGuiWindowFlags.NoDecoration;
        Flags |= ImGuiWindowFlags.NoBackground;
        Flags |= ImGuiWindowFlags.NoInputs;
        Flags |= ImGuiWindowFlags.NoBringToFrontOnFocus;
        Flags |= ImGuiWindowFlags.NoFocusOnAppearing;
        Flags |= ImGuiWindowFlags.NoNavFocus;

        ForceMainWindow = true;

        AnimationStopwatch.Start();

        var demoAction = LuminaCache<Action>.Instance.GetRow(67)!;

        DemoWarning = new WarningState
        {
            MessageShort = "Sample Warning",
            IconID = demoAction.Icon,
            IconLabel = "Sample",
            Priority = 11,
            MessageLong = "NoTankYou Sample Warning",
        };
    }

    private bool ShouldOpenWindow()
    {
        if (!PartyListAddon.DataAvailable) return false;

        if (!WarningCondition.ShouldShowWarnings()) return false;

        if (Settings.PreviewMode) return true;

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
        if (Settings.PreviewMode)
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
                    if (Settings.SoloMode && Service.ClientState.LocalPlayer?.ObjectId != playerCharacter.ObjectId) continue;

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
        if(Settings.JobIcon || Settings.PreviewMode)
            AnimateJobIcon(player);

        if(Settings.PlayerName || Settings.PreviewMode)
            AnimatePlayerName(player);

        if(Settings.WarningText || Settings.PreviewMode)
            AnimateWarningText(player, warning.MessageLong);
    }

    private void AnimateWarningText(PartyListAddonData partyMember, string warningText)
    {
        if (!Settings.FlashingEffects)
        {
            DrawText(partyMember, warningText, Settings.WarningTextColor.Value);
        }
        else
        {
            switch (AnimationStopwatch.ElapsedMilliseconds)
            {
                case < 500:
                    DrawText(partyMember, warningText, Colors.White);
                    break;

                case >= 500:
                    DrawText(partyMember, warningText, Settings.WarningTextColor.Value);
                    break;
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
        if (!Settings.FlashingEffects)
        {
            partyMember.UserInterface.SetIconVisibility(false);
            DrawWarningShield(partyMember);
        }
        else
        {
            switch (AnimationStopwatch.ElapsedMilliseconds)
            {
                case < 500:
                    partyMember.UserInterface.SetIconVisibility(true);
                    break;

                case >= 500:
                    partyMember.UserInterface.SetIconVisibility(false);
                    DrawWarningShield(partyMember);
                    break;
            }
        }
    }

    private void AnimatePlayerName(PartyListAddonData partyMember)
    {
        if (!Settings.FlashingEffects)
        {
            partyMember.UserInterface.SetPlayerNameOutlineColor(Settings.WarningOutlineColor.Value);
        }
        else
        {
            switch (AnimationStopwatch.ElapsedMilliseconds)
            {
                case < 500:
                    partyMember.UserInterface.SetPlayerNameOutlineColor(Vector4.Zero);
                    break;

                case >= 500:
                    partyMember.UserInterface.SetPlayerNameOutlineColor(Settings.WarningOutlineColor.Value);
                    break;
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
        player.UserInterface.SetIconVisibility(Condition.IsCrossWorld() || player.AgentData.ValidData);
        player.UserInterface.SetPlayerNameOutlineColor(Vector4.Zero);
    }
}