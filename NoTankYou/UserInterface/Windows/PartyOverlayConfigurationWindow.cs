using System;
using System.Numerics;
using Dalamud.Interface.Windowing;
using ImGuiNET;
using NoTankYou.Configuration;
using NoTankYou.Configuration.Components;
using NoTankYou.Localization;
using NoTankYou.UserInterface.Components.InfoBox;
using NoTankYou.Utilities;

namespace NoTankYou.UserInterface.Windows;

public class PartyOverlaySettings
{
    public Setting<bool> WarningText = new(true);
    public Setting<bool> PlayerName = new(true);
    public Setting<bool> JobIcon = new(true);
    public Setting<bool> FlashingEffects = new(true);
    public Setting<Vector4> WarningTextColor = new (Colors.SoftRed);
    public Setting<Vector4> WarningOutlineColor = new (Colors.Red);
    public Setting<bool> PreviewMode = new(true);
    public Setting<bool> SoloMode = new(false);
}

internal class PartyOverlayConfigurationWindow : Window, IDisposable
{
    private static PartyOverlaySettings Settings => Service.ConfigurationManager.CharacterConfiguration.PartyOverlay;

    public PartyOverlayConfigurationWindow() : base($"{Strings.TabItems.PartyOverlay.ConfigurationLabel} - {Service.ConfigurationManager.CharacterConfiguration.CharacterData.Name}" )
    {
        SizeConstraints = new WindowSizeConstraints
        {
            MinimumSize = new Vector2(275 * (4.0f / 3.0f), 350),
            MaximumSize = new Vector2(9999,9999)
        };

        Flags |= ImGuiWindowFlags.AlwaysVerticalScrollbar;

        Service.ConfigurationManager.OnCharacterDataAvailable += UpdateWindowTitle;
    }

    public void Dispose()
    {
        Service.ConfigurationManager.OnCharacterDataAvailable -= UpdateWindowTitle;
    }

    private void UpdateWindowTitle(object? sender, CharacterConfiguration e)
    {
        WindowName = $"{Strings.TabItems.PartyOverlay.ConfigurationLabel} - {e.CharacterData.Name}";
    }

    public override void Draw()
    {
        InfoBox.Instance
            .AddTitle(Strings.Configuration.PreviewMode)
            .AddConfigCheckbox(Strings.Configuration.PreviewMode, Settings.PreviewMode)
            .Draw();

        InfoBox.Instance
            .AddTitle(Strings.TabItems.BannerOverlay.SoloMode)
            .AddConfigCheckbox(Strings.TabItems.BannerOverlay.SoloMode, Settings.SoloMode, Strings.TabItems.BannerOverlay.SoloModeHelp)
            .Draw();

        InfoBox.Instance
            .AddTitle(Strings.Common.Labels.DisplayOptions)
            .AddConfigCheckbox(Strings.TabItems.PartyOverlay.JobIcon, Settings.JobIcon)
            .AddConfigCheckbox(Strings.TabItems.PartyOverlay.PlayerName, Settings.PlayerName)
            .AddConfigCheckbox(Strings.TabItems.PartyOverlay.WarningText, Settings.WarningText)
            .AddConfigCheckbox(Strings.TabItems.PartyOverlay.FlashingEffects, Settings.FlashingEffects)
            .Draw();

        InfoBox.Instance
            .AddTitle(Strings.TabItems.PartyOverlay.ColorOptions)
            .AddConfigColor(Strings.TabItems.PartyOverlay.WarningText, Settings.WarningTextColor)
            .AddConfigColor(Strings.TabItems.PartyOverlay.WarningOutlineColor, Settings.WarningOutlineColor)
            .Draw();
    }
}