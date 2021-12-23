using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ImGuiNET;

namespace NoTankYou.SettingsSystem.SubSettings
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
                Service.Configuration.EnableDancePartnerBanner = true;
                Service.Configuration.EnableKardionBanner = true;
                Service.Configuration.EnableFaerieBanner = true;
                Service.Configuration.EnableTankStanceBanner = true;
                Service.Configuration.EnableSummonerBanner = true;
            }

            ImGui.SameLine();

            if (ImGui.Button("Disable All", new(100, 25)))
            {
                Service.Configuration.EnableDancePartnerBanner = false;
                Service.Configuration.EnableKardionBanner = false;
                Service.Configuration.EnableFaerieBanner = false;
                Service.Configuration.EnableTankStanceBanner = false;
                Service.Configuration.EnableSummonerBanner = false;
            }

            ImGui.Spacing();
        }
    }
}
