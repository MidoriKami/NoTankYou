using ImGuiNET;

namespace NoTankYou.SettingsSystem.SettingsCategories
{
    internal class TankStanceSettings : SettingsCategory
    {
        private readonly Configuration.ModuleSettings Settings;

        public TankStanceSettings() : base("Tank Stance Settings")
        {
            Settings = Service.Configuration.TankStanceSettings;
        }

        protected override void DrawContents()
        {
            DrawTankStanceTab();
        }

        private void DrawTankStanceTab()
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
