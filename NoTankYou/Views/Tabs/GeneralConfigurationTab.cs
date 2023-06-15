using KamiLib.AutomaticUserInterface;
using KamiLib.Interfaces;
using NoTankYou.System;

namespace NoTankYou.UserInterface.Tabs;

public class GeneralConfigurationTab : ITabItem
{
    public string TabName => "General";
    public bool Enabled => true;
    public void Draw() => DrawableAttribute.DrawAttributes(NoTankYouSystem.SystemConfig);
}