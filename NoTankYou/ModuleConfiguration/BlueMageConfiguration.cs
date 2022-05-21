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
    internal class BlueMageConfiguration : ICustomConfigurable
    {
        public ModuleType ModuleType => ModuleType.BlueMage;

        public string ConfigurationPaneLabel => Strings.Modules.BlueMage.ConfigurationPanelLabel;
        public string AboutInformationBox => Strings.Modules.BlueMage.Description;
        public string TechnicalInformation => Strings.Modules.BlueMage.TechnicalDescription;
        public TextureWrap? AboutImage { get; }
        public GenericSettings GenericSettings => Settings;
        private static BlueMageModuleSettings Settings => Service.Configuration.ModuleSettings.BlueMage;

        public BlueMageConfiguration()
        {
            AboutImage = Image.LoadImage("Unavailable");
        }

        public void DrawTabItem()
        {
            ImGui.TextColored(Settings.Enabled ? Colors.SoftGreen : Colors.SoftRed, Strings.Modules.BlueMage.Label);
        }

        public void DrawOptions()
        {
        }
    }
}
