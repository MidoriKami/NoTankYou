using System.Numerics;
using Dalamud.Interface.Windowing;
using ImGuiNET;
using NoTankYou.Configuration.Overlays;
using NoTankYou.Localization;
using NoTankYou.UserInterface.Components.InfoBox;

namespace NoTankYou.UserInterface.Windows;

internal class PartyOverlayConfigurationWindow : Window
{
    private static PartyOverlaySettings Settings => Service.ConfigurationManager.CharacterConfiguration.PartyOverlay;

    private readonly InfoBox Options = new();
    private readonly InfoBox DisplayOptions = new();
    private readonly InfoBox ColorOptions = new();
    private readonly InfoBox PreviewMode = new();

    public PartyOverlayConfigurationWindow() : base($"{Strings.TabItems.PartyOverlay.ConfigurationLabel} - {Service.ConfigurationManager.CharacterConfiguration.CharacterData.Name}" )
    {
        SizeConstraints = new WindowSizeConstraints
        {
            MinimumSize = new Vector2(350 * (4.0f / 3.0f), 350),
            MaximumSize = new Vector2(9999,9999)
        };

        Flags |= ImGuiWindowFlags.AlwaysVerticalScrollbar;
    }

    public override void Draw()
    {
        PreviewMode
            .AddTitle(Strings.Configuration.PreviewMode)
            .AddConfigCheckbox(Strings.Configuration.PreviewMode, Settings.PreviewMode)
            .Draw();

        Options
            .AddTitle(Strings.Common.Labels.Options)
            .AddConfigCheckbox(Strings.Configuration.HideInSanctuary, Settings.DisableInSanctuary)
            .Draw();

        DisplayOptions
            .AddTitle(Strings.Common.Labels.DisplayOptions)
            .AddConfigCheckbox(Strings.TabItems.PartyOverlay.JobIcon, Settings.JobIcon)
            .AddConfigCheckbox(Strings.TabItems.PartyOverlay.PlayerName, Settings.PlayerName)
            .AddConfigCheckbox(Strings.TabItems.PartyOverlay.WarningText, Settings.WarningText)
            .AddConfigCheckbox(Strings.TabItems.PartyOverlay.FlashingEffects, Settings.FlashingEffects)
            .Draw();

        ColorOptions
            .AddTitle(Strings.TabItems.PartyOverlay.ColorOptions)
            .AddConfigColor(Strings.TabItems.PartyOverlay.WarningText, Settings.WarningTextColor)
            .AddConfigColor(Strings.TabItems.PartyOverlay.WarningOutlineColor, Settings.WarningOutlineColor)
            .Draw();
    }
}