using KamiLib.InfoBoxSystem;
using NoTankYou.Configuration.Components;
using NoTankYou.Localization;

namespace NoTankYou.Utilities;

public static class InfoBoxExtensions
{
    public static void DrawGenericSettings(this InfoBox instance, GenericSettings settings)
    {
        instance
            .AddTitle(Strings.Common.Tabs.Settings)
            .AddConfigCheckbox(Strings.Common.Labels.Enabled, settings.Enabled)
            .AddConfigCheckbox(Strings.Configuration.SoloMode, settings.SoloMode, Strings.Configuration.SoloModeHelp)
            .AddConfigCheckbox(Strings.Configuration.DutiesOnly, settings.DutiesOnly, Strings.Configuration.DutiesOnlyHelp)
            .AddInputInt(Strings.Common.Labels.Priority, settings.Priority, 0, 10)
            .Draw();
    }

    public static void DrawOverlaySettings(this InfoBox instance, GenericSettings settings)
    {
        instance
            .AddTitle(Strings.Common.Labels.DisplayOptions)
            .AddConfigCheckbox(Strings.TabItems.PartyOverlay.Label, settings.PartyFrameOverlay)
            .AddConfigCheckbox(Strings.TabItems.BannerOverlay.Label, settings.BannerOverlay)
            .Draw();
    }

    public static void DrawOptions(this InfoBox instance, GenericSettings settings)
    {
        instance
            .AddTitle(Strings.Common.Labels.Options)
            .AddConfigCheckbox(Strings.Configuration.HideInSanctuary, settings.DisableInSanctuary)
            .Draw();
    }
}