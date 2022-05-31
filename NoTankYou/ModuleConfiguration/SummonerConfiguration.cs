using ImGuiNET;
using NoTankYou.Data.Components;
using NoTankYou.Data.Modules;
using NoTankYou.Enums;
using NoTankYou.Interfaces;
using NoTankYou.Localization;
using NoTankYou.Utilities;

namespace NoTankYou.ModuleConfiguration
{
    internal class SummonerConfiguration : ICustomConfigurable
    {
        public ModuleType ModuleType => ModuleType.Summoner;
        public string ConfigurationPaneLabel => Strings.Modules.Summoner.ConfigurationPanelLabel;
        public GenericSettings GenericSettings => Settings;
        private static SummonerModuleSettings Settings => Service.Configuration.ModuleSettings.Summoner;
        public void DrawTabItem()
        {
            ImGui.TextColored(Settings.Enabled ? Colors.SoftGreen : Colors.SoftRed, Strings.Modules.Summoner.Label);
        }

        public void DrawOptions()
        {

        }
    }
}
