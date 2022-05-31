using ImGuiNET;
using NoTankYou.Data.Components;
using NoTankYou.Data.Modules;
using NoTankYou.Enums;
using NoTankYou.Interfaces;
using NoTankYou.Localization;
using NoTankYou.Utilities;

namespace NoTankYou.ModuleConfiguration
{
    internal class SageConfiguration : ICustomConfigurable
    {
        public ModuleType ModuleType => ModuleType.Sage;
        public string ConfigurationPaneLabel => Strings.Modules.Sage.ConfigurationPanelLabel;
        public GenericSettings GenericSettings => Settings;
        private static SageModuleSettings Settings => Service.Configuration.ModuleSettings.Sage;

        public void DrawTabItem()
        {
            ImGui.TextColored(Settings.Enabled ? Colors.SoftGreen : Colors.SoftRed, Strings.Modules.Sage.Label);
        }

        public void DrawOptions()
        {

        }
    }
}
