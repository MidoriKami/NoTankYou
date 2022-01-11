using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NoTankYou.SettingsSystem.SubSettings.General;

namespace NoTankYou.SettingsSystem.SettingsCategories
{
    internal class DisplaySettings : TabCategory
    {
        private readonly SelectSizeSettings SelectSizeSettings = new();
        private readonly RepositionSettings RepositionSettings = new();

        public DisplaySettings()
        {
            CategoryName = "Display Settings";
            TabName = "Display";
        }

        protected override void DrawContents()
        {
            SelectSizeSettings.Draw();

            RepositionSettings.Draw();
        }
    }
}
