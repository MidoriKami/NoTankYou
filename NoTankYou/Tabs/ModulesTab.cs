using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NoTankYou.Interfaces;
using NoTankYou.Localization;
using NoTankYou.ModuleConfiguration;

namespace NoTankYou.Tabs
{
    internal class ModulesTab : ITab
    {
        public ITabItem? SelectedTabItem { get; set; }
        public List<ITabItem> TabItems { get; set; } = new()
        {
            new TanksConfiguration(),
            new DancerConfiguration(),
            new FoodConfiguration(),
        };

        public string TabName => Strings.Common.Tabs.Modules;
        public string Description => Strings.Common.Tabs.ModulesDescription;
    }
}
