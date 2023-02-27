using ImGuiNET;
using KamiLib.Configuration;
using KamiLib.Drawing;
using KamiLib.Interfaces;
using NoTankYou.DataModels;
using NoTankYou.Localization;

namespace NoTankYou.UserInterface.Components;

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

public class BannerOverlayConfigurationSelectable : ISelectable, IDrawable
{
    public IDrawable Contents => this;
    public string ID => Strings.BannerOverlay_Label;
    
    private static BannerOverlaySettings Settings => Service.ConfigurationManager.CharacterConfiguration.BannerOverlay;
    
    public void DrawLabel() => ImGui.Text(ID);

    public void Draw()
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
            .AddTitle(Strings.Labels_DisplayOptions, out var innerWidth)
            .AddConfigCheckbox(Strings.BannerOverlay_ExclamationMark, Settings.WarningShield)
            .AddConfigCheckbox(Strings.BannerOverlay_WarningText, Settings.WarningText)
            .AddConfigCheckbox(Strings.BannerOverlay_PlayerNames, Settings.PlayerNames)
            .AddConfigCheckbox(Strings.BannerOverlay_Icon, Settings.Icon)
            .AddConfigCheckbox(Strings.BannerOverlay_IconText, Settings.IconText)
            .AddString(Strings.BannerOverlay_BorderThickness + ":")
            .AddDragFloat("##BorderThickness", Settings.BorderThickness, 0.5f, 3.0f, innerWidth)
            .Draw();

        InfoBox.Instance
            .AddTitle(Strings.Labels_Scale, out var scaleWidth)
            .AddDragFloat("", Settings.Scale, 0.1f, 5.0f, scaleWidth)
            .Draw();

        InfoBox.Instance
            .AddTitle(Strings.Labels_ModeSelect)
            .BeginTable()
            .BeginRow()
            .AddConfigRadio(Strings.BannerOverlay_ListMode, Settings.Mode, BannerOverlayDisplayMode.List, Strings.BannerOverlay_ListModeDescription)
            .AddConfigRadio(Strings.BannerOverlay_TopPriorityMode, Settings.Mode, BannerOverlayDisplayMode.TopPriority, Strings.BannerOverlay_TopPriorityDescription)
            .EndRow()
            .EndTable()
            .Draw();

        if (Settings.Mode == BannerOverlayDisplayMode.List)
        {
            InfoBox.Instance
                .AddTitle(Strings.BannerOverlay_ListModeOptions, out var listModeWidth)
                .AddString(Strings.BannerOverlay_WarningCount + ":")
                .AddSliderInt("##PlayerWarningCount", Settings.WarningCount, 1, 8, listModeWidth)
                .Draw();
        }
    }
}