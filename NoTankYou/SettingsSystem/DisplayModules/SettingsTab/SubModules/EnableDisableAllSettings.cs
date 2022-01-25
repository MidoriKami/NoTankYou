using Dalamud.Interface;
using ImGuiNET;

namespace NoTankYou.SettingsSystem.DisplayModules.SettingsTab.SubModules
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
            if (ImGui.Button("Enable All", ImGuiHelpers.ScaledVector2(100, 25)))
            {
                foreach (var (name, element) in SettingsModules.Modules)
                {
                    element.Enabled = true;
                }
            }

            ImGui.SameLine();

            if (ImGui.Button("Disable All", ImGuiHelpers.ScaledVector2(100, 25)))
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
