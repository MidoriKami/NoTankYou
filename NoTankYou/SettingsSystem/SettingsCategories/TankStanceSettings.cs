using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ImGuiNET;

namespace NoTankYou.SettingsSystem.SettingsCategories
{
    internal class TankStanceSettings : SettingsCategory
    {
        public TankStanceSettings() : base("Tank Stance Settings")
        {
        }

        protected override void DrawContents()
        {
            DrawTankStanceTab();
        }

        private static void DrawTankStanceTab()
        {
            ImGui.Checkbox("Enable Tank Stance Warning", ref Service.Configuration.TankStanceSettings.Enabled);
            ImGui.Spacing();

            ImGui.Checkbox("Force Show Banner", ref Service.Configuration.TankStanceSettings.Forced);
            ImGui.Spacing();

            ImGui.Checkbox("Reposition Banner", ref Service.Configuration.TankStanceSettings.Reposition);
            ImGui.Spacing();
        }
    }
}
