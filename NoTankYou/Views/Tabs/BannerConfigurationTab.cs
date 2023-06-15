using KamiLib.Interfaces;
using NoTankYou.System;

namespace NoTankYou.UserInterface.Tabs;

public class BannerConfigurationTab : ITabItem
{
    public string TabName => "Banner Overlay";
    public bool Enabled => true;
    public void Draw()
    {
        NoTankYouSystem.BannerController.DrawConfig();
    }
}