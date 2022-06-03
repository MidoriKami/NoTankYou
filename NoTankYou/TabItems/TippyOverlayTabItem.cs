using System;
using Dalamud.Interface;
using Dalamud.Interface.Components;
using ImGuiNET;
using NoTankYou.Components;
using NoTankYou.Data.Overlays;
using NoTankYou.Enums;
using NoTankYou.Interfaces;
using NoTankYou.Localization;
using NoTankYou.Utilities;

namespace NoTankYou.TabItems
{
    internal class TippyOverlayTabItem : IConfigurable
    {
        public ModuleType ModuleType => ModuleType.TippyOverlay;
        public string ConfigurationPaneLabel => Strings.TabItems.TippyDisplay.ConfigurationLabel;
        public static TippyOverlaySettings Settings => Service.Configuration.DisplaySettings.TippyOverlay;

        private readonly InfoBox WarningFrequency = new()
        {
            Label = Strings.TabItems.TippyDisplay.WarningFrequency,
            ContentsAction = () =>
            {
                ImGui.SetNextItemWidth(50 * ImGuiHelpers.GlobalScale);
                ImGui.InputInt(Strings.Common.Labels.Seconds, ref Settings.WarningFrequency, 0, 0);
                if (ImGui.IsItemDeactivatedAfterEdit())
                {
                    Settings.WarningFrequency = Math.Clamp(Settings.WarningFrequency, 10, 600);
                    Service.Configuration.Save();
                }
            }
        };

        private readonly InfoBox Options = new()
        {
            Label = Strings.Common.Labels.Options,
            ContentsAction = () =>
            {
                if (ImGui.Checkbox(Strings.Configuration.Enable, ref Settings.Enabled))
                {
                    Service.Configuration.Save();
                }

                if (ImGui.Checkbox(Strings.Configuration.SoloMode, ref Settings.SoloMode))
                {
                    Service.Configuration.Save();
                }

                if (ImGui.Checkbox(Strings.Configuration.DutiesOnly, ref Settings.DutiesOnly))
                {
                    Service.Configuration.Save();
                }
                ImGuiComponents.HelpMarker(Strings.Configuration.DutiesOnlyHelp);

                if (Settings.Enabled)
                {
                    if (ImGui.Checkbox(Strings.Configuration.HideInSanctuary, ref Settings.DisableInSanctuary))
                    {
                        Service.Configuration.Save();
                    }
                }
            }
        };

        public void DrawTabItem()
        {
            ImGui.TextColored(Settings.Enabled ? Colors.SoftGreen : Colors.SoftRed,
                Strings.TabItems.TippyDisplay.Label);
        }

        public void DrawOptions()
        {
            if (TippyInstalled())
            {
                ImGuiHelpers.ScaledDummy(10.0f);
                Options.DrawCentered();

                ImGuiHelpers.ScaledDummy(30.0f);
                WarningFrequency.DrawCentered();

                ImGuiHelpers.ScaledDummy(20.0f);
            }
            else
            {
                var region = ImGui.GetContentRegionAvail();
                
                var firstLine = Strings.TabItems.TippyDisplay.TippyNotInstalled;
                var firstLineSize = ImGui.CalcTextSize(firstLine);

                var secondLine = Strings.TabItems.TippyDisplay.TippyNotInstalledInstructions;
                var secondLineSize = ImGui.CalcTextSize(secondLine);

                ImGuiHelpers.ScaledDummy(20.0f);
                var cursorPosition = ImGui.GetCursorPos();
                var firstLinePosition = cursorPosition with {X = region.X /2.0f - firstLineSize.X / 2.0f };

                ImGui.SetCursorPos(firstLinePosition);
                ImGui.TextColored(Colors.SoftRed, firstLine);
                
                ImGuiHelpers.ScaledDummy(20.0f);
                cursorPosition = ImGui.GetCursorPos();
                var secondLinePosition = cursorPosition with {X = region.X / 2.0f - secondLineSize.X / 2.0f };

                ImGui.SetCursorPos(secondLinePosition);
                ImGui.TextColored(Colors.SoftRed, secondLine);
            }
        }

        private bool TippyInstalled()
        {
            return Service.PluginInterface.PluginInternalNames.Contains("Tippy");
        }
    }
}
