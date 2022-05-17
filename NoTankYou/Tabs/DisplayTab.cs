using System.Collections.Generic;
using NoTankYou.DisplayTabItems;
using NoTankYou.Interfaces;
using NoTankYou.Localization;

namespace NoTankYou.Tabs
{
    internal class DisplayTab : ITab
    {
        public ITabItem? SelectedTabItem { get; set; }
        public List<ITabItem> TabItems { get; set; } = new()
        {
            new PartyOverlayTabItem(),
        };

        public string TabName => Strings.Common.Tabs.Display;
        public string Description => Strings.Common.Tabs.DisplayDescription;
    }
}
