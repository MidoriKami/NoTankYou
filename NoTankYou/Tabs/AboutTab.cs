using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NoTankYou.Interfaces;

namespace NoTankYou.Tabs
{
    internal class AboutTab : ITab
    {
        public ITabItem? SelectedTabItem { get; set; }
        public List<ITabItem> TabItems { get; set; } = new()
        {

        };

        public string TabName => "About";
        public string Description => "Basic information about No Tank You";
    }
}
