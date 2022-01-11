using System.Runtime.CompilerServices;
using ImGuiNET;

namespace NoTankYou.SettingsSystem.SubSettings.General
{
    internal class EnableDisableAllSettings : SettingsCategory
    {
        public EnableDisableAllSettings() : base("Enable/Disable All Banners")
        {
        }

        protected override void DrawContents()
        {
            DrawEnabledDisableAll();
        }

        private void DrawEnabledDisableAll()
        {
            if (ImGui.Button("Enable All", new(100, 25)))
            {
                foreach (var (name, element) in SettingsModules.Modules)
                {
                    element.Enabled = true;
                }
            }

            ImGui.SameLine();

            if (ImGui.Button("Disable All", new(100, 25)))
            {
                foreach (var (name, element) in SettingsModules.Modules)
                {
                    element.Enabled = false;
                }
            }

            ImGui.Spacing();
        }
    }
}
