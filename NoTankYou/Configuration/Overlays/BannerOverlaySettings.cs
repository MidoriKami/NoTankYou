using System;
using NoTankYou.Configuration.Components;
using NoTankYou.Localization;

namespace NoTankYou.Configuration.Overlays;

public enum BannerOverlayDisplayMode
{
    TopPriority,
    List
}

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
    public Setting<bool> DisableInSanctuary = new(false);
    public Setting<float> BorderThickness = new(1.0f);
}

internal static class BannerOverlayDisplayModeExtensions
{
    internal static string GetLabel(this BannerOverlayDisplayMode mode)
    {
        return mode switch
        {
            BannerOverlayDisplayMode.TopPriority => Strings.TabItems.BannerOverlay.TopPriorityMode,
            BannerOverlayDisplayMode.List => Strings.TabItems.BannerOverlay.ListMode,
            _ => throw new ArgumentOutOfRangeException(nameof(mode), mode, null)
        };
    }
}