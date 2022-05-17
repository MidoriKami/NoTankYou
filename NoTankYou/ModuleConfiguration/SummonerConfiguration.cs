using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dalamud.Interface;
using ImGuiNET;
using ImGuiScene;
using NoTankYou.Components;
using NoTankYou.Data.Components;
using NoTankYou.Data.Modules;
using NoTankYou.Enums;
using NoTankYou.Interfaces;
using NoTankYou.Localization;
using NoTankYou.Utilities;

namespace NoTankYou.ModuleConfiguration
{
    internal class SummonerConfiguration : IConfigurable
    {
        public ModuleType ModuleType => ModuleType.Summoner;
        public string ConfigurationPaneLabel => Strings.Modules.Summoner.ConfigurationPanelLabel;
        public string AboutInformationBox => Strings.Modules.Summoner.Description;
        public string TechnicalInformation => Strings.Modules.Summoner.TechnicalDescription;
        public TextureWrap? AboutImage { get; }
        public GenericSettings GenericSettings => Settings;
        private static SummonerModuleSettings Settings => Service.Configuration.ModuleSettings.Summoner;

        public SummonerConfiguration()
        {
            AboutImage = Image.LoadImage("Summoner");
        }

        public void DrawTabItem()
        {
            ImGui.TextColored(Settings.Enabled ? Colors.SoftGreen : Colors.SoftRed, Strings.Modules.Summoner.Label);
        }

        public void DrawOptions()
        {

        }
    }
}
