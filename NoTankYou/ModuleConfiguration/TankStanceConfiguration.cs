using ImGuiNET;
using ImGuiScene;
using NoTankYou.Components;
using NoTankYou.Enums;
using NoTankYou.Interfaces;
using NoTankYou.Localization;

namespace NoTankYou.ModuleConfiguration
{
    internal class TankStanceConfiguration : IConfigurable
    {
        public ModuleType ModuleType => ModuleType.TankStance;
        public string ConfigurationPaneLabel => Strings.Modules.Tank.ConfigurationPanelLabel;
        public InfoBox? AboutInformationBox { get; }
        public InfoBox? TechnicalInformation { get; }
        public TextureWrap? AboutImage { get; }

        public TankStanceConfiguration()
        {

        }

        public void DrawTabItem()
        {
            ImGui.Text(Strings.Modules.Tank.Label);
        }

        public void DrawOptionsContents()
        {

        }
    }
}
