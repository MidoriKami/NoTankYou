using ImGuiNET;
using System.Numerics;

namespace NoTankYou.SettingsSystem.SubSettings.General
{
    internal class RepositionSettings : SettingsCategory
    {
        private bool CurrentlyRepositioningAll = false;

        public RepositionSettings() : base("Reposition Banner Settings")
        {
            if (Service.Configuration.DancePartnerSettings.Reposition == true &&
                Service.Configuration.KardionSettings.Reposition == true &&
                Service.Configuration.FaerieSettings.Reposition == true &&
                Service.Configuration.TankStanceSettings.Reposition == true &&
                Service.Configuration.SummonerSettings.Reposition == true &&
                Service.Configuration.BlueMageSettings.Reposition == true)
            {
                CurrentlyRepositioningAll = true;
            }
            else
            {
                CurrentlyRepositioningAll = false;
            }
        }

        protected override void DrawContents()
        {
            DrawRepositionAll();
            DrawRepositionAllStatus();
            DrawEnableSnapping();
            DrawSnappingHint();
        }

        private void DrawRepositionAll()
        {
            if (ImGui.Button("Enable Reposition All", new(150, 25)))
            {
                Service.Configuration.DancePartnerSettings.Reposition = true;
                Service.Configuration.KardionSettings.Reposition = true;
                Service.Configuration.FaerieSettings.Reposition = true;
                Service.Configuration.TankStanceSettings.Reposition = true;
                Service.Configuration.BlueMageSettings.Reposition = true;
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
                Service.Configuration.BlueMageSettings.Reposition = false;
                Service.Configuration.SummonerSettings.Reposition = false;
                CurrentlyRepositioningAll = false;
            }

            ImGui.Spacing();
        }

        private void DrawRepositionAllStatus()
        {
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

            ImGui.Spacing();
        }

        private void DrawEnableSnapping()
        {
            ImGui.Checkbox("Enable Window Snapping", ref Service.Configuration.WindowSnappingEnabled);
            ImGui.Spacing();
        }

        private void DrawSnappingHint()
        {
            ImGui.TextColored(new Vector4(120, 0, 0, 0.4f), "Press and hold [Control] while dragging a banner to snap");
            ImGui.Spacing();
        }
    }
}
