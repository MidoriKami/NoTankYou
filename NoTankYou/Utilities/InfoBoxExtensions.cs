using KamiLib.Drawing;
using NoTankYou.DataModels;
using NoTankYou.Localization;

namespace NoTankYou.Utilities;

public static class InfoBoxExtensions
{
    public static void DrawGenericSettings(this InfoBox instance, GenericSettings settings)
    {
        instance
            .AddTitle(Strings.Tabs_Settings)
            .AddConfigCheckbox(Strings.Labels_Enabled, settings.Enabled)
            .AddConfigCheckbox(Strings.BannerOverlay_SoloMode, settings.SoloMode, Strings.Configuration_SoloModeHelp)
            .AddConfigCheckbox(Strings.Configuration_DutiesOnly, settings.DutiesOnly, Strings.Configuration_DutiesOnlyHelp)
            .AddInputInt(Strings.Labels_Priority, settings.Priority, 0, 10)
            .Draw();
    }

    public static void DrawOverlaySettings(this InfoBox instance, GenericSettings settings)
    {
        instance
            .AddTitle(Strings.Labels_DisplayOptions)
            .AddConfigCheckbox(Strings.PartyOverlay_Label, settings.PartyFrameOverlay)
            .AddConfigCheckbox(Strings.BannerOverlay_Label, settings.BannerOverlay)
            .Draw();
    }

    public static void DrawOptions(this InfoBox instance, GenericSettings settings)
    {
        instance
            .AddTitle(Strings.Labels_Options)
            .AddConfigCheckbox(Strings.Configuration_HideInSanctuary, settings.DisableInSanctuary)
            .Draw();
    }
}