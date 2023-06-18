using KamiLib.Interfaces;
using NoTankYou.Localization;
using NoTankYou.System;

namespace NoTankYou.UserInterface.Tabs;

public class BlacklistConfigurationTab : ITabItem
{
    public string TabName => Strings.Blacklist;
    public bool Enabled => true;
    public void Draw() => NoTankYouSystem.BlacklistController.DrawConfig();
}