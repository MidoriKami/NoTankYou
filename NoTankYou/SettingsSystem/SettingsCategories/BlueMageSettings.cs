using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NoTankYou.SettingsSystem.SettingsCategories
{
    internal class BlueMageSettings : BannerSettings
    {
        public BlueMageSettings()
        {
            CategoryName = "Blue Mage Settings";
            TabName = "BLU";
        }

        protected override ref Configuration.ModuleSettings Settings => ref Service.Configuration.BlueMageSettings;
    }
}
