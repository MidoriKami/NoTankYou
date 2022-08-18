using System;
using Dalamud.Interface;
using ImGuiNET;
using NoTankYou.Components;
using NoTankYou.Data.Components;
using NoTankYou.Data.Modules;
using NoTankYou.Enums;
using NoTankYou.Interfaces;
using NoTankYou.Localization;
using NoTankYou.Utilities;

namespace NoTankYou.ModuleConfiguration {
    internal class SpiritbondConfiguration : ICustomConfigurable {
        public ModuleType ModuleType => Enums.ModuleType.Spiritbond;
        public string ConfigurationPaneLabel => Strings.Modules.Spiritbond.ConfigurationPanelLabel;
        public GenericSettings GenericSettings => Settings;
        public static SpiritbondModuleSettings Settings => Service.Configuration.ModuleSettings.Spiritbond;

        private readonly InfoBox EarlyWarningTime = new () {
            Label = Strings.Modules.Spiritbond.EarlyWarningLabel,
            ContentsAction = () => {
                ImGui.SetNextItemWidth(75.0f * ImGuiHelpers.GlobalScale);
                ImGui.InputInt(Strings.Common.Labels.Seconds, ref Settings.SpiritbondEarlyWarningTime, 0, 0);
                if (ImGui.IsItemDeactivatedAfterEdit()) {
                    Settings.SpiritbondEarlyWarningTime = Math.Clamp(Settings.SpiritbondEarlyWarningTime, 0, 3600);
                }
            }
        };

        private readonly InfoBox ZoneFilters = new () {
            Label = Strings.Modules.Spiritbond.ZoneFilters,
            ContentsAction = () => {
                ImGui.Text(Strings.Modules.Spiritbond.ZoneFiltersDescription);
                ImGui.Spacing();
                if (ImGui.Checkbox(Strings.Common.Labels.Savage, ref Settings.SavageDuties)) {
                    Service.Configuration.Save();
                }

                if (ImGui.Checkbox(Strings.Common.Labels.Ultimate, ref Settings.UltimateDuties)) {
                    Service.Configuration.Save();
                }

                if (ImGui.Checkbox(Strings.Common.Labels.ExtremeUnreal, ref Settings.ExtremeUnreal)) {
                    Service.Configuration.Save();
                }
            }
        };

        private readonly InfoBox AdditionalOptions = new () {
            Label = Strings.Modules.Spiritbond.AdditionalOptionsLabel,
            ContentsAction = () => {
                if (ImGui.Checkbox(Strings.Modules.Spiritbond.SuppressInCombat, ref Settings.DisableInCombat)) {
                    Service.Configuration.Save();
                }
            }
        };

        public void DrawTabItem() {
            ImGui.TextColored(Settings.Enabled ? Colors.SoftGreen : Colors.SoftRed, Strings.Modules.Spiritbond.Label);
        }

        public void DrawOptions() {
            ZoneFilters.DrawCentered();
            ImGuiHelpers.ScaledDummy(30.0f);
            AdditionalOptions.DrawCentered();
            ImGuiHelpers.ScaledDummy(30.0f);
            EarlyWarningTime.DrawCentered();
        }
    }
}