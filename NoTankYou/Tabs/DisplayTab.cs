using System.Collections.Generic;
using NoTankYou.Interfaces;
using NoTankYou.Localization;
using NoTankYou.TabItems;

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
