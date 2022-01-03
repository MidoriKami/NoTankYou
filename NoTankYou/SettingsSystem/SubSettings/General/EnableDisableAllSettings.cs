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

        private static void DrawEnabledDisableAll()
        {
            if (ImGui.Button("Enable All", new(100, 25)))
            {
                Service.Configuration.DancePartnerSettings.Enabled = true;
                Service.Configuration.KardionSettings.Enabled = true;
                Service.Configuration.FaerieSettings.Enabled = true;
                Service.Configuration.TankStanceSettings.Enabled = true;
                Service.Configuration.SummonerSettings.Enabled = true;
                Service.Configuration.BlueMageSettings.Enabled = true;
            }

            ImGui.SameLine();

            if (ImGui.Button("Disable All", new(100, 25)))
            {
                Service.Configuration.DancePartnerSettings.Enabled = false;
                Service.Configuration.KardionSettings.Enabled = false;
                Service.Configuration.FaerieSettings.Enabled = false;
                Service.Configuration.TankStanceSettings.Enabled = false;
                Service.Configuration.SummonerSettings.Enabled = false;
                Service.Configuration.BlueMageSettings.Enabled = false;
            }

            ImGui.Spacing();
        }
    }
}
