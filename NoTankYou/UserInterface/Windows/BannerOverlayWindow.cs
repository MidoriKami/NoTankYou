﻿using System.Linq;
using System.Numerics;
using Dalamud.Game.ClientState.Objects.SubKinds;
using Dalamud.Interface;
using Dalamud.Interface.Windowing;
using Dalamud.Utility.Signatures;
using ImGuiNET;
using ImGuiScene;
using NoTankYou.Configuration.Components;
using NoTankYou.Utilities;
using Condition = NoTankYou.Utilities.Condition;

namespace NoTankYou.UserInterface.Windows;

internal class BannerOverlayWindow : Window
{
    private static BannerOverlaySettings Settings => Service.ConfigurationManager.CharacterConfiguration.BannerOverlay;
    private readonly TextureWrap WarningIcon;

    private int WarningsDisplayed;
    private readonly WarningState DemoWarning;

    private delegate bool IsInSanctuary();

    [Signature("E8 ?? ?? ?? ?? 84 C0 75 21 48 8B 4F 10")]
    private readonly IsInSanctuary SanctuaryFunction = null!;

    private bool InSanctuaryArea;

    public BannerOverlayWindow() : base($"###BannerOverlay+{Service.ConfigurationManager.CharacterConfiguration.CharacterData.Name}")
    {
        SignatureHelper.Initialise(this);

        Service.WindowSystem.AddWindow(this);

        WarningIcon = Image.LoadImage("BigWarning");

        ForceMainWindow = true;

        Flags |= ImGuiWindowFlags.NoDecoration;
        Flags |= ImGuiWindowFlags.NoBackground;
        Flags |= ImGuiWindowFlags.NoInputs;
        Flags |= ImGuiWindowFlags.NoBringToFrontOnFocus;
        Flags |= ImGuiWindowFlags.NoFocusOnAppearing;
        Flags |= ImGuiWindowFlags.NoNavFocus;

        DemoWarning = new WarningState
        {
            MessageShort = "Sample Warning",
            IconID = 786,
            IconLabel = "Sample",
            Priority = 11,
            MessageLong = "Long Sample Warning"
        };
    }

    public override void PreOpenCheck()
    {
        IsOpen = Condition.ShouldShowWarnings() || Settings.SampleMode.Value;

        InSanctuaryArea = SanctuaryFunction();
    }

    public override void PreDraw()
    {
        Size = ImGuiHelpers.ScaledVector2(500.0f, 90.0f) * Settings.Scale.Value;

        var backgroundColor = ImGui.GetStyle().Colors[(int)ImGuiCol.WindowBg];
        ImGui.PushStyleColor(ImGuiCol.WindowBg, backgroundColor with {W = 0.35f});
    }

    public override void Draw()
    {
        WarningsDisplayed = 0;

        if (Settings.SampleMode.Value)
        {
            Flags &= ~ImGuiWindowFlags.NoInputs;
            Flags &= ~ImGuiWindowFlags.NoBackground;

            if (Service.ClientState.LocalPlayer is { } player)
            {
                DrawWarningStateBanner(DemoWarning, player);

                var position = ImGui.GetWindowPos() - ImGuiHelpers.ScaledVector2(0.0f, 30.0f * Settings.Scale.Value);
                DrawUtilities.TextOutlined(position, "Open NoTankYou Settings to Configure Warnings", 0.5f * Settings.Scale.Value, Colors.Orange);
            }
        }
        else
        {
            Flags |= ImGuiWindowFlags.NoInputs;
            Flags |= ImGuiWindowFlags.NoBackground;

            foreach (var player in Service.PartyListAddon)
            {
                if (player.IsTargetable() && player.PlayerCharacter is { } playerCharacter)
                {
                    // Get all Logic Modules for this classjob
                    var modules = Service.ModuleManager.GetModulesForClassJob(playerCharacter.ClassJob.Id);

                    // Filter to only modules that are enabled for PartyFrame Overlay
                    var enabledModules = modules
                        .Where(module => module.ParentModule.GenericSettings.BannerOverlay.Value)
                        .Where(module => !(module.ParentModule.GenericSettings.DisableInSanctuary.Value && InSanctuaryArea));

                    // Get Highest Warning for remaining modules
                    var highestWarning = enabledModules
                        .Select(module => module.ShouldShowWarning(playerCharacter))
                        .OfType<WarningState>()
                        .DefaultIfEmpty(null)
                        .Aggregate((i1, i2) => i1!.Priority > i2!.Priority ? i1 : i2);

                    // If the warning exists
                    if (highestWarning is not null)
                    {
                        DrawWarningStateBanner(highestWarning, playerCharacter);
                    }
                }
            }
        }
    }

    public override void PostDraw()
    {
        ImGui.PopStyleColor();
    }

    private void DrawWarningStateBanner(WarningState state, PlayerCharacter player)
    {
        var mode = Settings.SampleMode.Value ? BannerOverlayDisplayMode.TopPriority : Settings.Mode.Value;

        switch (mode)
        {
            case BannerOverlayDisplayMode.TopPriority when WarningsDisplayed == 0:
                DrawBanner(ImGui.GetWindowPos(), state.MessageShort, player.Name.TextValue, Settings.Scale.Value, state.IconID, state.IconLabel);
                WarningsDisplayed = 1;
                break;

            case BannerOverlayDisplayMode.List when WarningsDisplayed < Settings.WarningCount.Value:
                var adjustedPosition = ImGui.GetWindowPos() + new Vector2(0.0f, 80.0f) * Settings.Scale.Value * WarningsDisplayed;
                DrawBanner(adjustedPosition, state.MessageShort, player.Name.TextValue, Settings.Scale.Value, state.IconID, state.IconLabel);
                WarningsDisplayed += 1;
                break;
        }
    }

    private void DrawBanner(Vector2 startPosition, string warningText, string source, float scale, uint iconId,
        string skillName)
    {
        var drawList = ImGui.GetBackgroundDrawList();

        var startingPosition = startPosition;

        if (Settings.WarningShield.Value)
        {
            var imageSize = new Vector2(WarningIcon.Width, WarningIcon.Height) * scale;

            drawList.AddImage(WarningIcon.ImGuiHandle, startingPosition, startingPosition + imageSize);
            startingPosition = startingPosition with
            {
                X = startingPosition.X + imageSize.X + 10.0f * scale, Y = startingPosition.Y + 4.0f
            };
        }

        if (Settings.WarningText.Value)
        {
            DrawUtilities.TextOutlined(startingPosition, warningText, scale, Colors.White);

            var textSize = DrawUtilities.CalculateTextSize(warningText, scale);
            startingPosition = startingPosition with {X = startingPosition.X + textSize.X};

            if (Settings.PlayerNames.Value)
            {
                textSize = DrawUtilities.CalculateTextSize(source, scale);
                startingPosition = startingPosition with
                {
                    X = startingPosition.X - textSize.X / 2.0f, Y = startingPosition.Y + textSize.Y
                };
                DrawUtilities.TextOutlined(startingPosition, source, scale / 2.0f, Colors.White);
                startingPosition = startingPosition with
                {
                    X = startingPosition.X + textSize.X / 2.0f, Y = startingPosition.Y - textSize.Y
                };
            }
        }

        if (Settings.Icon.Value)
            DrawUtilities.DrawIconWithName(startingPosition, iconId, skillName, scale, Settings.IconText.Value);
    }
}