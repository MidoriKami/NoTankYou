using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ImGuiNET;

namespace NoTankYou.SettingsSystem.SettingsCategories
{
    internal class SummonerPetSettings : SettingsCategory
    {
        public SummonerPetSettings() : base("Summoner Pet Settings")
        {
        }

        protected override void DrawContents()
        {
            DrawSummonerTab();
        }
        private void DrawSummonerTab()
        {
            ImGui.Checkbox("Enable Missing Summoner Pet Warning", ref Service.Configuration.SummonerSettings.Enabled);
            ImGui.Spacing();

            ImGui.Checkbox("Force Show Banner", ref Service.Configuration.SummonerSettings.Forced);
            ImGui.Spacing();

            ImGui.Checkbox("Reposition Banner", ref Service.Configuration.SummonerSettings.Reposition);
            ImGui.Spacing();
        }
    }
}
