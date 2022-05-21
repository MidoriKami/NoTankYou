using System;
using Dalamud.Interface;
using Dalamud.Interface.Components;
using ImGuiNET;
using ImGuiScene;
using NoTankYou.Components;
using NoTankYou.Data.Components;
using NoTankYou.Data.Modules;
using NoTankYou.Enums;
using NoTankYou.Interfaces;
using NoTankYou.Localization;
using NoTankYou.Utilities;

namespace NoTankYou.ModuleConfiguration
{
    internal class BlueMageConfiguration : IConfigurable
    {
        public ModuleType ModuleType => ModuleType.BlueMage;

        public string ConfigurationPaneLabel => Strings.Modules.BlueMage.ConfigurationPanelLabel;
        public string AboutInformationBox => Strings.Modules.BlueMage.Description;
        public string TechnicalInformation => Strings.Modules.BlueMage.TechnicalDescription;
        public TextureWrap? AboutImage { get; }
        private static BlueMageModuleSettings Settings => Service.Configuration.ModuleSettings.BlueMage;

        private readonly InfoBox Warnings = new()
        {
            Label = Strings.Common.Labels.Warnings,
            ContentsAction = () =>
            {
                if (ImGui.Checkbox(Strings.Modules.BlueMage.MimicryLabel, ref Settings.Mimicry))
                {
                    if (Settings.Mimicry)
                    {
                        Settings.DutiesOnly = false;
                    }
                    Service.Configuration.Save();
                }
                ImGuiComponents.HelpMarker(Strings.Modules.BlueMage.MimicryWarning);

                if (ImGui.Checkbox(Strings.Modules.BlueMage.MightyGuardLabel, ref Settings.TankStance))
                {
                    Service.Configuration.Save();
                }

                if (ImGui.Checkbox(Strings.Modules.BlueMage.BasicInstinctLabel, ref Settings.BasicInstinct))
                {
                    Service.Configuration.Save();
                }
            }
        };

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
            ImGuiHelpers.ScaledDummy(10.0f);
            new InfoBox
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
                        Settings.Mimicry = false;
                        Service.Configuration.Save();
                    }
                    ImGuiComponents.HelpMarker(Strings.Configuration.DutiesOnlyHelp);
                }
            }.DrawCentered();

            ImGuiHelpers.ScaledDummy(30.0f);
            new InfoBox
            {
                Label = Strings.Common.Labels.Priority,
                ContentsAction = () =>
                {
                    ImGui.SetNextItemWidth(75.0f * ImGuiHelpers.GlobalScale);
                    ImGui.InputInt("", ref Settings.Priority, 1, 1);
                    if (ImGui.IsItemDeactivatedAfterEdit())
                    {
                        Settings.Priority = Math.Clamp(Settings.Priority, 0, 10);
                    }
                }
            }.DrawCentered();
            
            ImGuiHelpers.ScaledDummy(30.0f);
            Warnings.DrawCentered();

            ImGuiHelpers.ScaledDummy(20.0f);
        }
    }
}
