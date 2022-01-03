using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NoTankYou.SettingsSystem.SubSettings.General;

namespace NoTankYou.SettingsSystem.SettingsCategories
{
    internal class DisplaySettings : SettingsCategory
    {
        private readonly SelectSizeSettings SelectSizeSettings = new();
        private readonly RepositionSettings RepositionSettings = new();

        public DisplaySettings() : base("Display")
        {
        }

        protected override void DrawContents()
        {
            SelectSizeSettings.Draw();

            RepositionSettings.Draw();
        }
    }
}
