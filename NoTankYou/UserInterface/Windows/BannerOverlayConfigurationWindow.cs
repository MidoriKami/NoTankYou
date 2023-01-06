using System;
using System.Numerics;
using Dalamud.Interface.Windowing;
using ImGuiNET;
using KamiLib.Configuration;
using KamiLib.InfoBoxSystem;
using NoTankYou.Configuration;
using NoTankYou.DataModels;
using NoTankYou.Localization;

namespace NoTankYou.UserInterface.Windows;

public class BannerOverlaySettings
{
    public Setting<float> Scale = new (1.0f);
    public Setting<int> WarningCount = new(8);
    public Setting<BannerOverlayDisplayMode> Mode = new(BannerOverlayDisplayMode.List);
    public Setting<bool> SampleMode = new(true);
    public Setting<bool> WarningShield = new(true);
    public Setting<bool> WarningText = new(true);
    public Setting<bool> Icon = new(true);
    public Setting<bool> PlayerNames = new(true);
    public Setting<bool> IconText = new(true);
    public Setting<float> BorderThickness = new(1.0f);
    public Setting<bool> SoloMode = new(false);
}

public class BannerOverlayConfigurationWindow : Window, IDisposable
{
    private static BannerOverlaySettings Settings => Service.ConfigurationManager.CharacterConfiguration.BannerOverlay;
    
    public BannerOverlayConfigurationWindow() : base($"{Strings.BannerOverlay_ConfigurationLabel} - {Service.ConfigurationManager.CharacterConfiguration.CharacterData.Name}")
    {
        SizeConstraints = new WindowSizeConstraints
        {
            MinimumSize = new Vector2(320,590),
            MaximumSize = new Vector2(320,590),
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
        WindowName = $"{Strings.BannerOverlay_ConfigurationLabel} - {e.CharacterData.Name}";
    }

    public override void Draw()
    {
        InfoBox.Instance
            .AddTitle(Strings.Configuration_PreviewMode)
            .AddConfigCheckbox(Strings.BannerOverlay_RepositionMode, Settings.SampleMode)
            .Draw();

        InfoBox.Instance
            .AddTitle(Strings.BannerOverlay_SoloMode)
            .AddConfigCheckbox(Strings.BannerOverlay_SoloMode, Settings.SoloMode, Strings.BannerOverlay_SoloModeHelp)
            .Draw();

        InfoBox.Instance
            .AddTitle(Strings.Labels_DisplayOptions)
            .AddConfigCheckbox(Strings.BannerOverlay_ExclamationMark, Settings.WarningShield)
            .AddConfigCheckbox(Strings.BannerOverlay_WarningText, Settings.WarningText)
            .AddConfigCheckbox(Strings.BannerOverlay_PlayerNames, Settings.PlayerNames)
            .AddConfigCheckbox(Strings.BannerOverlay_Icon, Settings.Icon)
            .AddConfigCheckbox(Strings.BannerOverlay_IconText, Settings.IconText)
            .AddString(Strings.BannerOverlay_BorderThickness + ":")
            .AddDragFloat("##BorderThickness", Settings.BorderThickness, 0.5f, 3.0f, InfoBox.Instance.InnerWidth)
            .Draw();

        InfoBox.Instance
            .AddTitle(Strings.Labels_Scale)
            .AddDragFloat("", Settings.Scale, 0.1f, 5.0f, InfoBox.Instance.InnerWidth)
            .Draw();

        InfoBox.Instance
            .AddTitle(Strings.Labels_ModeSelect)
                .BeginTable()
                    .BeginRow()
                    .AddConfigRadio(Strings.BannerOverlay_ListMode, Settings.Mode, BannerOverlayDisplayMode.List, Strings.BannerOverlay_ListModeDescription)
                    .AddConfigRadio(Strings.BannerOverlay_TopPriorityMode, Settings.Mode, BannerOverlayDisplayMode.TopPriority, Strings.BannerOverlay_TopPriorityDescription)
                    .EndRow()
                .EndTable()
            .SameLine()
            .Draw();

        if (Settings.Mode == BannerOverlayDisplayMode.List)
        {
            InfoBox.Instance
                .AddTitle(Strings.BannerOverlay_ListModeOptions)
                .AddString(Strings.BannerOverlay_WarningCount + ":")
                .AddSliderInt("##PlayerWarningCount", Settings.WarningCount, 1, 8, InfoBox.Instance.InnerWidth)
                .Draw();
        }
    }
}