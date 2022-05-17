using Dalamud.Interface;
using ImGuiNET;
using ImGuiScene;
using NoTankYou.Components;
using NoTankYou.Data.Modules;
using NoTankYou.Enums;
using NoTankYou.Interfaces;
using NoTankYou.Localization;
using NoTankYou.Utilities;
using NoTankYou.Windows.PartyFrameOverlayWindow;

namespace NoTankYou.ModuleConfiguration
{
    internal class TanksConfiguration : IConfigurable
    {
        public ModuleType ModuleType => ModuleType.Tanks;
        public string ConfigurationPaneLabel => Strings.Modules.Tank.ConfigurationPanelLabel;
        public InfoBox? AboutInformationBox { get; }
        public InfoBox? TechnicalInformation { get; }
        public TextureWrap? AboutImage { get; }
        private static TankModuleSettings Settings => Service.Configuration.ModuleSettings.Tank;

        public TanksConfiguration()
        {

        }

        public void DrawTabItem()
        {
            ImGui.TextColored(Settings.Enabled ? Colors.SoftGreen : Colors.SoftRed, Strings.Modules.Tank.Label);
        }

        public readonly InfoBox Options = new()
        {
            Label = Strings.Common.Labels.Options,
            ContentsAction = () =>
            {
                if (ImGui.Checkbox(Strings.Configuration.Enable, ref Settings.Enabled))
                {
                    Service.Configuration.Save();
                }
            }
        };

        public void DrawOptionsContents()
        {
            ImGuiHelpers.ScaledDummy(10.0f);
            Options.DrawCentered();

            ImGuiHelpers.ScaledDummy(30.0f);

            ImGuiHelpers.ScaledDummy(20.0f);

        }
    }
}
