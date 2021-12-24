using ImGuiNET;

namespace NoTankYou.SettingsSystem.SettingsCategories
{
    internal class KardionSettings : SettingsCategory
    {
        private readonly Configuration.ModuleSettings Settings;

        public KardionSettings() : base("Kardion Settings")
        {
            Settings = Service.Configuration.KardionSettings;
        }

        protected override void DrawContents()
        {
            DrawKardionTab();
        }

        private void DrawKardionTab()
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
