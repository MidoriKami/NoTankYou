using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using ImGuiNET;

namespace NoTankYou.SettingsSystem.SubSettings
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
                Service.Configuration.RepositionModeDancePartnerBanner = true;
                Service.Configuration.RepositionModeKardionBanner = true;
                Service.Configuration.RepositionModeFaerieBanner = true;
                Service.Configuration.RepositionModeTankStanceBanner = true;
                Service.Configuration.RepositionModeSummonerBanner = true;
                CurrentlyRepositioningAll = true;
            }

            ImGui.SameLine();

            if (ImGui.Button("Disable Reposition All", new(150, 25)))
            {
                Service.Configuration.RepositionModeDancePartnerBanner = false;
                Service.Configuration.RepositionModeKardionBanner = false;
                Service.Configuration.RepositionModeFaerieBanner = false;
                Service.Configuration.RepositionModeTankStanceBanner = false;
                Service.Configuration.RepositionModeSummonerBanner = false;
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
