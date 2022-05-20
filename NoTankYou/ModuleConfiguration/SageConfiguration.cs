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
    internal class SageConfiguration : IConfigurable
    {
        public ModuleType ModuleType => ModuleType.Sage;
        public string ConfigurationPaneLabel => Strings.Modules.Sage.ConfigurationPanelLabel;
        public string AboutInformationBox => Strings.Modules.Sage.Description;
        public string TechnicalInformation => Strings.Modules.Sage.TechnicalDescription;
        public TextureWrap? AboutImage { get; }
        public GenericSettings GenericSettings => Settings;
        private static SageModuleSettings Settings => Service.Configuration.ModuleSettings.Sage;

        public SageConfiguration()
        {
            AboutImage = Image.LoadImage("Sage");
        }

        public void DrawTabItem()
        {
            ImGui.TextColored(Settings.Enabled ? Colors.SoftGreen : Colors.SoftRed, Strings.Modules.Sage.Label);
        }

        public void DrawOptions()
        {

        }
    }
}
