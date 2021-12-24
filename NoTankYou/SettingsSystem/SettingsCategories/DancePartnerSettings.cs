using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ImGuiNET;

namespace NoTankYou.SettingsSystem.SettingsCategories
{
    internal class DancePartnerSettings : SettingsCategory
    {
        public DancePartnerSettings() : base("Dance Partner Settings")
        {
        }

        protected override void DrawContents()
        {
            DrawDancePartnerTab();
        }

        private static void DrawDancePartnerTab()
        {
            ImGui.Checkbox("Enable Missing Dance Partner Warning", ref Service.Configuration.DancePartnerSettings.Enabled);
            ImGui.Spacing();

            ImGui.Checkbox("Force Show Banner", ref Service.Configuration.DancePartnerSettings.Forced);
            ImGui.Spacing();

            ImGui.Checkbox("Reposition Banner", ref Service.Configuration.DancePartnerSettings.Reposition);
            ImGui.Spacing();
        }
    }
}
