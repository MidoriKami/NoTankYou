using System.Numerics;
using ImGuiNET;

namespace NoTankYou.SettingsSystem.SubSettings.General
{
    internal class RepositionSettings : SettingsCategory
    {
        private bool CurrentlyRepositioningAll = false;

        public RepositionSettings() : base("Reposition Banner Settings")
        {
        }

        protected override void DrawContents()
        {
            DrawRepositionAll();

        }

        private void DrawRepositionAll()
        {
            if (ImGui.Button("Enable Reposition All", new(150, 25)))
            {
                Service.Configuration.DancePartnerSettings.Reposition = true;
                Service.Configuration.KardionSettings.Reposition = true;
                Service.Configuration.FaerieSettings.Reposition = true;
                Service.Configuration.TankStanceSettings.Reposition = true;
                Service.Configuration.SummonerSettings.Reposition = true;
                CurrentlyRepositioningAll = true;
            }

            ImGui.SameLine();

            if (ImGui.Button("Disable Reposition All", new(150, 25)))
            {
                Service.Configuration.DancePartnerSettings.Reposition = false;
                Service.Configuration.KardionSettings.Reposition = false;
                Service.Configuration.FaerieSettings.Reposition = false;
                Service.Configuration.TankStanceSettings.Reposition = false;
                Service.Configuration.SummonerSettings.Reposition = false;
                CurrentlyRepositioningAll = false;
            }

            ImGui.Spacing();

            ImGui.Text("Reposition all: ");

            ImGui.SameLine();

            if (CurrentlyRepositioningAll)
            {
                ImGui.TextColored(new Vector4(0, 255, 0, 0.8f), "Enabled");
            }
            else
            {
                ImGui.TextColored(new Vector4(185, 0, 0, 0.8f), "Disabled");
            }
        }
    }
}
