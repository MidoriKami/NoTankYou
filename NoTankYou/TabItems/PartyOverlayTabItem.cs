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
using NoTankYou.Windows.PartyFrameOverlayWindow;

namespace NoTankYou.TabItems
{
    internal class PartyOverlayTabItem : IConfigurable
    {
        public ModuleType ModuleType => ModuleType.PartyOverlay;
        public string ConfigurationPaneLabel => Strings.TabItems.PartyOverlay.Label;

        public string AboutInformationBox => Strings.TabItems.PartyOverlay.Description;

        public string TechnicalInformation => Strings.TabItems.PartyOverlay.TechnicalDescription;

        private readonly InfoBox Options = new()
        {
            Label = Strings.Common.Labels.Options,
            ContentsAction = () =>
            {
                if (ImGui.Checkbox(Strings.Configuration.Enable, ref Settings.Enabled))
                {
                    if (!Settings.Enabled)
                    {
                        PartyFrameOverlayWindow.ResetAllAnimation();
                    }

                    Service.Configuration.Save();
                }
            }
        };

        private readonly InfoBox DisplayOptions = new()
        {
            Label = Strings.TabItems.PartyOverlay.DisplayOptions,
            ContentsAction = () =>
            {
                if (ImGui.Checkbox(Strings.TabItems.PartyOverlay.JobIcon, ref Settings.JobIcon))
                {
                    PartyFrameOverlayWindow.ResetAllAnimation();
                    Service.Configuration.Save();
                }

                if (ImGui.Checkbox(Strings.TabItems.PartyOverlay.PlayerName, ref Settings.PlayerName))
                {
                    PartyFrameOverlayWindow.ResetAllAnimation();
                    Service.Configuration.Save();
                }

                if (ImGui.Checkbox(Strings.TabItems.PartyOverlay.WarningText, ref Settings.WarningText))
                {
                    PartyFrameOverlayWindow.ResetAllAnimation();
                    Service.Configuration.Save();
                }                

                ImGuiHelpers.ScaledDummy(10.0f);
                
                if (ImGui.Checkbox(Strings.TabItems.PartyOverlay.FlashingEffects, ref Settings.FlashingEffects))
                {
                    PartyFrameOverlayWindow.ResetAllAnimation();
                    Service.Configuration.Save();
                }
            }
        };

        public TextureWrap? AboutImage { get; }
        public GenericSettings GenericSettings => null!;
        private static PartyOverlaySettings Settings => Service.Configuration.DisplaySettings.PartyOverlay;

        public PartyOverlayTabItem()
        {
            AboutImage = Image.LoadImage("PartyOverlay");
        }

        public void DrawTabItem()
        {
            ImGui.TextColored(Settings.Enabled ? Colors.SoftGreen : Colors.SoftRed, Strings.TabItems.PartyOverlay.Label);
        }

        public void DrawOptions()
        {
            ImGuiHelpers.ScaledDummy(10.0f);
            Options.DrawCentered();

            ImGuiHelpers.ScaledDummy(30.0f);
            DisplayOptions.DrawCentered();

            ImGuiHelpers.ScaledDummy(20.0f);
        }
    }
}
