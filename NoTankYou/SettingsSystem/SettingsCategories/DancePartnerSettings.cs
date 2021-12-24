using ImGuiNET;

namespace NoTankYou.SettingsSystem.SettingsCategories
{
    internal class DancePartnerSettings : SettingsCategory
    {
        private readonly Configuration.ModuleSettings Settings;

        public DancePartnerSettings() : base("Dance Partner Settings")
        {
            Settings = Service.Configuration.DancePartnerSettings;
        }

        protected override void DrawContents()
        {
            DrawDancePartnerTab();
        }

        private void DrawDancePartnerTab()
        {
            ImGui.Checkbox("Enable Missing Dance Partner Warning", ref Settings.Enabled);
            ImGui.Spacing();

            ImGui.Checkbox("Force Show Banner", ref Settings.Forced);
            ImGui.Spacing();

            ImGui.Checkbox("Reposition Banner", ref Settings.Reposition);
            ImGui.Spacing();
        }
    }
}
