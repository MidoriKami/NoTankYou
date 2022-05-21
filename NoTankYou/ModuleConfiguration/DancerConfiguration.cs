using ImGuiNET;
using ImGuiScene;
using NoTankYou.Data.Components;
using NoTankYou.Data.Modules;
using NoTankYou.Enums;
using NoTankYou.Interfaces;
using NoTankYou.Localization;
using NoTankYou.Utilities;

namespace NoTankYou.ModuleConfiguration
{
    internal class DancerConfiguration : ICustomConfigurable
    {
        public ModuleType ModuleType => ModuleType.Dancer;
        public string ConfigurationPaneLabel => Strings.Modules.Dancer.ConfigurationPanelLabel;
        public string AboutInformationBox => Strings.Modules.Dancer.Description;
        public string TechnicalInformation => Strings.Modules.Dancer.TechnicalDescription;
        public TextureWrap? AboutImage { get; }
        public GenericSettings GenericSettings => Settings;
        private static DancerModuleSettings Settings => Service.Configuration.ModuleSettings.Dancer;

        public DancerConfiguration()
        {
            AboutImage = Image.LoadImage("Unavailable");
        }

        public void DrawTabItem()
        {
            ImGui.TextColored(Settings.Enabled ? Colors.SoftGreen : Colors.SoftRed, Strings.Modules.Dancer.Label);
        }

        public void DrawOptions()
        {
        }
    }
}
