using KamiLib.Interfaces;
using NoTankYou.Localization;
using NoTankYou.System;

namespace NoTankYou.UserInterface.Tabs;

public class BannerConfigurationTab : ITabItem
{
    public string TabName => Strings.BannerOverlay;
    public bool Enabled => true;
    public void Draw() => NoTankYouSystem.BannerController.DrawConfig();
}