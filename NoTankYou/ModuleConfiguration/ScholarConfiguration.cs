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
    internal class ScholarConfiguration : ICustomConfigurable
    {
        public ModuleType ModuleType => ModuleType.Scholar;
        public string ConfigurationPaneLabel => Strings.Modules.Scholar.ConfigurationPanelLabel;
        public string AboutInformationBox => Strings.Modules.Scholar.Description;
        public string TechnicalInformation => Strings.Modules.Scholar.TechnicalDescription;
        public TextureWrap? AboutImage { get; }
        public GenericSettings GenericSettings => Settings;
        private static ScholarModuleSettings Settings => Service.Configuration.ModuleSettings.Scholar;

        public ScholarConfiguration()
        {
            AboutImage = Image.LoadImage("Scholar");
        }

        public void DrawTabItem()
        {
            ImGui.TextColored(Settings.Enabled ? Colors.SoftGreen : Colors.SoftRed, Strings.Modules.Scholar.Label);
        }

        public void DrawOptions()
        {

        }
    }
}
