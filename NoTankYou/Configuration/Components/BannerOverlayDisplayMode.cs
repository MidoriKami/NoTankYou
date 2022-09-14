using System;
using NoTankYou.Localization;

namespace NoTankYou.Configuration.Components;

public enum BannerOverlayDisplayMode
{
    TopPriority,
    List
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