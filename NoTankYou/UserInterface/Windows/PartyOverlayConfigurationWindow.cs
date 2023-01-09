using System;
using System.Numerics;
using Dalamud.Interface.Windowing;
using ImGuiNET;
using KamiLib.Configuration;
using KamiLib.Drawing;
using NoTankYou.Configuration;
using NoTankYou.Localization;

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

public class PartyOverlayConfigurationWindow : Window, IDisposable
{
    private static PartyOverlaySettings Settings => Service.ConfigurationManager.CharacterConfiguration.PartyOverlay;

    public PartyOverlayConfigurationWindow() : base($"{Strings.PartyOverlay_ConfigurationLabel} - {Service.ConfigurationManager.CharacterConfiguration.CharacterData.Name}" )
    {
        SizeConstraints = new WindowSizeConstraints
        {
            MinimumSize = new Vector2(250,400),
            MaximumSize = new Vector2(250,400),
        };

        Flags |= ImGuiWindowFlags.NoResize;

        Service.ConfigurationManager.OnCharacterDataAvailable += UpdateWindowTitle;
    }

    public void Dispose()
    {
        Service.ConfigurationManager.OnCharacterDataAvailable -= UpdateWindowTitle;
    }

    private void UpdateWindowTitle(object? sender, CharacterConfiguration e)
    {
        WindowName = $"{Strings.PartyOverlay_ConfigurationLabel} - {e.CharacterData.Name}";
    }

    public override void Draw()
    {
        InfoBox.Instance
            .AddTitle(Strings.Configuration_PreviewMode)
            .AddConfigCheckbox(Strings.Configuration_PreviewMode, Settings.PreviewMode)
            .Draw();

        InfoBox.Instance
            .AddTitle(Strings.BannerOverlay_SoloMode)
            .AddConfigCheckbox(Strings.BannerOverlay_SoloMode, Settings.SoloMode, Strings.BannerOverlay_SoloModeHelp)
            .Draw();

        InfoBox.Instance
            .AddTitle(Strings.Labels_DisplayOptions)
            .AddConfigCheckbox(Strings.PartyOverlay_JobIcon, Settings.JobIcon)
            .AddConfigCheckbox(Strings.PartyOverlay_PlayerName, Settings.PlayerName)
            .AddConfigCheckbox(Strings.PartyOverlay_WarningText, Settings.WarningText)
            .AddConfigCheckbox(Strings.PartyOverlay_FlashingEffects, Settings.FlashingEffects)
            .Draw();

        InfoBox.Instance
            .AddTitle(Strings.PartyOverlay_ColorOptions)
            .AddConfigColor(Strings.PartyOverlay_WarningText, Strings.Labels_Default, Settings.WarningTextColor, Colors.SoftRed)
            .AddConfigColor(Strings.PartyOverlay_WarningOutlineColor, Strings.Labels_Default, Settings.WarningOutlineColor, Colors.Red)
            .Draw();
    }
}