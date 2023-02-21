using System.Numerics;
using ImGuiNET;
using KamiLib.Configuration;
using KamiLib.Drawing;
using KamiLib.Interfaces;
using NoTankYou.Localization;

namespace NoTankYou.UserInterface.Components;

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

public class PartyOverlayConfigurationSelectable : ISelectable, IDrawable
{
    public IDrawable Contents => this;

    public string ID => Strings.PartyOverlay_Label;
    
    private static PartyOverlaySettings Settings => Service.ConfigurationManager.CharacterConfiguration.PartyOverlay;
    
    public void DrawLabel() => ImGui.Text(ID);

    public void Draw()
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