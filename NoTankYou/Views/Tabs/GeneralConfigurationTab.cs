using KamiLib.Interfaces;
using NoTankYou.Localization;

namespace NoTankYou.UserInterface.Tabs;

public class GeneralConfigurationTab : ITabItem
{
    public string TabName => Strings.General;
    public bool Enabled => true;
    public void Draw() => NoTankYouPlugin.System.DrawConfig();
}