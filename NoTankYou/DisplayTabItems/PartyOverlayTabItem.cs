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

namespace NoTankYou.DisplayTabItems
{
    internal class PartyOverlayTabItem : IConfigurable
    {
        public ModuleType ModuleType => ModuleType.PartyOverlay;
        public string ConfigurationPaneLabel => Strings.Common.TabItems.PartyOverlay.Label;

        public InfoBox? AboutInformationBox { get; } = new()
        {
            Label = Strings.Common.Labels.About,
            ContentsAction = () =>
            {
                ImGui.Text(Strings.Common.TabItems.PartyOverlay.Description);
            }
        };

        public InfoBox? TechnicalInformation { get; } = new()
        {
            Label = Strings.Common.Labels.TechnicalDescription,
            ContentsAction = () =>
            {
                ImGui.Text(Strings.Common.TabItems.PartyOverlay.TechnicalDescription);
            }
        };

        public readonly InfoBox Options = new()
        {
            Label = Strings.Common.Labels.Options,
            ContentsAction = () =>
            {
                if (ImGui.Checkbox(Strings.Configuration.Enable, ref Settings.Enabled))
                {
                    if (!Settings.Enabled)
                    {
                        Service.WindowManager.GetWindowOfType<PartyFrameOverlayWindow>()?.ResetAllAnimation();
                    }

                    Service.Configuration.Save();
                }
            }
        };

        public TextureWrap? AboutImage { get; }

        private static PartyOverlaySettings Settings => Service.Configuration.DisplaySettings.PartyOverlay;

        public PartyOverlayTabItem()
        {
            AboutImage = Image.LoadImage("PartyOverlay");
        }

        public void DrawTabItem()
        {
            ImGui.TextColored(Settings.Enabled ? Colors.SoftGreen : Colors.SoftRed, Strings.Common.TabItems.PartyOverlay.Label);
        }

        public void DrawOptionsContents()
        {
            ImGuiHelpers.ScaledDummy(10.0f);
            Options.DrawCentered();

            ImGuiHelpers.ScaledDummy(30.0f);

            ImGuiHelpers.ScaledDummy(20.0f);

        }
    }
}
