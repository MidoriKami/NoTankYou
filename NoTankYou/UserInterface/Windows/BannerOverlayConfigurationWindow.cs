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
    
    public BannerOverlayConfigurationWindow() : base($"{Strings.TabItems.BannerOverlay.ConfigurationLabel} - {Service.ConfigurationManager.CharacterConfiguration.CharacterData.Name}")
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
        WindowName = $"{Strings.TabItems.BannerOverlay.ConfigurationLabel} - {e.CharacterData.Name}";
    }

    public override void Draw()
    {
        InfoBox.Instance
            .AddTitle(Strings.Configuration.PreviewMode)
            .AddConfigCheckbox(Strings.TabItems.BannerOverlay.RepositionMode, Settings.SampleMode)
            .Draw();

        InfoBox.Instance
            .AddTitle(Strings.TabItems.BannerOverlay.SoloMode)
            .AddConfigCheckbox(Strings.TabItems.BannerOverlay.SoloMode, Settings.SoloMode, Strings.TabItems.BannerOverlay.SoloModeHelp)
            .Draw();

        InfoBox.Instance
            .AddTitle(Strings.Common.Labels.DisplayOptions)
            .AddConfigCheckbox(Strings.TabItems.BannerOverlay.ExclamationMark, Settings.WarningShield)
            .AddConfigCheckbox(Strings.TabItems.BannerOverlay.WarningText, Settings.WarningText)
            .AddConfigCheckbox(Strings.TabItems.BannerOverlay.PlayerNames, Settings.PlayerNames)
            .AddConfigCheckbox(Strings.TabItems.BannerOverlay.Icon, Settings.Icon)
            .AddConfigCheckbox(Strings.TabItems.BannerOverlay.IconText, Settings.IconText)
            .AddString(Strings.TabItems.BannerOverlay.BorderThickness + ":")
            .AddDragFloat("##BorderThickness", Settings.BorderThickness, 0.5f, 3.0f, InfoBox.Instance.InnerWidth)
            .Draw();

        InfoBox.Instance
            .AddTitle(Strings.Common.Labels.Scale)
            .AddDragFloat("", Settings.Scale, 0.1f, 5.0f, InfoBox.Instance.InnerWidth)
            .Draw();

        InfoBox.Instance
            .AddTitle(Strings.Common.Labels.ModeSelect)
                .BeginTable()
                    .BeginRow()
                    .AddConfigRadio(Strings.TabItems.BannerOverlay.ListMode, Settings.Mode, BannerOverlayDisplayMode.List, Strings.TabItems.BannerOverlay.ListModeDescription)
                    .AddConfigRadio(Strings.TabItems.BannerOverlay.TopPriorityMode, Settings.Mode, BannerOverlayDisplayMode.TopPriority, Strings.TabItems.BannerOverlay.TopPriorityDescription)
                    .EndRow()
                .EndTable()
            .SameLine()
            .Draw();

        if (Settings.Mode == BannerOverlayDisplayMode.List)
        {
            InfoBox.Instance
                .AddTitle(Strings.TabItems.BannerOverlay.ListModeOptions)
                .AddString(Strings.TabItems.BannerOverlay.WarningCount + ":")
                .AddSliderInt("##PlayerWarningCount", Settings.WarningCount, 1, 8, InfoBox.Instance.InnerWidth)
                .Draw();
        }
    }
}