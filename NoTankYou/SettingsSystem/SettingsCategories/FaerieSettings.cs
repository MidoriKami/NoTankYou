using ImGuiNET;

namespace NoTankYou.SettingsSystem.SettingsCategories
{
    internal class FaerieSettings : SettingsCategory
    {
        private readonly Configuration.ModuleSettings Settings;

        public FaerieSettings() : base("Faerie Settings")
        {
            Settings = Service.Configuration.FaerieSettings;
        }

        protected override void DrawContents()
        {
            DrawFaerieTab();
        }

        private void DrawFaerieTab()
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
