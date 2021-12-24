using ImGuiNET;

namespace NoTankYou.SettingsSystem.SettingsCategories
{
    internal class SummonerPetSettings : SettingsCategory
    {
        private readonly Configuration.ModuleSettings Settings;

        public SummonerPetSettings() : base("Summoner Pet Settings")
        {
            Settings = Service.Configuration.SummonerSettings;
        }

        protected override void DrawContents()
        {
            DrawSummonerTab();
        }
        private void DrawSummonerTab()
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
