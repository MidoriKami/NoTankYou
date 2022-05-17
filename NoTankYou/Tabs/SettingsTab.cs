using System.Collections.Generic;
using NoTankYou.Interfaces;
using NoTankYou.Localization;
using NoTankYou.ModuleConfiguration;

namespace NoTankYou.Tabs
{
    internal class SettingsTab : ITab
    {
        public ITabItem? SelectedTabItem { get; set; }
        public List<ITabItem> TabItems { get; set; } = new()
        {
        };

        public string TabName => Strings.Common.Tabs.Settings;
        public string Description => Strings.Common.Tabs.SettingsDescription;
    }
}
