using KamiLib.Interfaces;
using NoTankYou.System;

namespace NoTankYou.UserInterface.Tabs;

public class PartyListOverlayConfigurationTab : ITabItem
{
    public string TabName => "Party List Overlay";
    public bool Enabled => true;
    public void Draw() => NoTankYouSystem.PartyListController.DrawConfig();
}