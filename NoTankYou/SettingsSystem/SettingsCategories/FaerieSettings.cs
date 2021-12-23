using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ImGuiNET;

namespace NoTankYou.SettingsSystem.SettingsCategories
{
    internal class FaerieSettings : SettingsCategory
    {
        public FaerieSettings() : base("Faerie Settings")
        {
        }

        protected override void DrawContents()
        {
            DrawFaerieTab();
        }

        private static void DrawFaerieTab()
        {
            ImGui.Checkbox("Enable Missing Faerie Warning", ref Service.Configuration.EnableFaerieBanner);
            ImGui.Spacing();

            ImGui.Checkbox("Force Show Banner", ref Service.Configuration.ForceShowFaerieBanner);
            ImGui.Spacing();

            ImGui.Checkbox("Reposition Banner", ref Service.Configuration.RepositionModeFaerieBanner);
            ImGui.Spacing();
        }
    }
}
