using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NoTankYou.DisplaySystem;
using NoTankYou.SettingsSystem.SubSettings.Positioning;

namespace NoTankYou.SettingsSystem.SettingsCategories
{
    internal class PositioningSettings : SettingsCategory
    {
        private readonly VerticalAlignmentSettings VerticalAlignmentSettings = new();
        private readonly HorizontalAlignmentSettings HorizontalAlignmentSettings = new();

        public PositioningSettings() : base("Positioning Settings")
        {
        }

        protected override void DrawContents()
        {
            VerticalAlignmentSettings.Draw();

            HorizontalAlignmentSettings.Draw();
        }
    }
}
