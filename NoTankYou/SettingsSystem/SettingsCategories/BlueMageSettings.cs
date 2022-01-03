using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NoTankYou.SettingsSystem.SettingsCategories
{
    internal class BlueMageSettings : BannerSettings
    {
        public BlueMageSettings() : base("Blue Mage Settings")
        {
        }

        protected override ref Configuration.ModuleSettings Settings => ref Service.Configuration.BlueMageSettings;
    }
}
