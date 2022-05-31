using System;
using Dalamud.Interface;
using Dalamud.Interface.Components;
using ImGuiNET;
using NoTankYou.Components;
using NoTankYou.Data.Components;
using NoTankYou.Localization;

namespace NoTankYou.Interfaces
{
    internal interface ICustomConfigurable : ITabItem
    {
        string ConfigurationPaneLabel { get; }
        GenericSettings GenericSettings { get; }

        void DrawOptions();

        void ITabItem.DrawConfigurationPane()
        {
            var contentWidth = ImGui.GetContentRegionAvail().X;
            var textWidth = ImGui.CalcTextSize(ConfigurationPaneLabel).X;
            var textStart = contentWidth / 2.0f - textWidth / 2.0f;

            ImGui.SetCursorPos(ImGui.GetCursorPos() with {X = textStart});
            ImGui.Text(ConfigurationPaneLabel);

            ImGui.Spacing();

            if (ImGui.BeginChild("OptionsContentsChild", ImGui.GetContentRegionAvail(), false, ImGuiWindowFlags.AlwaysVerticalScrollbar))
            {
                DrawBaseOptions();
            }
            ImGui.EndChild();
        }

        void DrawBaseOptions()
        {
            ImGuiHelpers.ScaledDummy(10.0f);
            new InfoBox
            {
                Label = Strings.Common.Labels.Options,
                ContentsAction = () =>
                {
                    if (ImGui.Checkbox(Strings.Configuration.Enable, ref GenericSettings.Enabled))
                    {
                        Service.Configuration.Save();
                    }

                    if (ImGui.Checkbox(Strings.Configuration.SoloMode, ref GenericSettings.SoloMode))
                    {
                        Service.Configuration.Save();
                    }
                    ImGuiComponents.HelpMarker(Strings.Configuration.SoloModeHelp);

                    if (ImGui.Checkbox(Strings.Configuration.DutiesOnly, ref GenericSettings.DutiesOnly))
                    {
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
                    ImGui.InputInt("", ref GenericSettings.Priority, 1, 1);
                    if (ImGui.IsItemDeactivatedAfterEdit())
                    {
                        GenericSettings.Priority = Math.Clamp(GenericSettings.Priority, 0, 10);
                    }
                }
            }.DrawCentered();
            
            ImGuiHelpers.ScaledDummy(30.0f);
            DrawOptions();

            ImGuiHelpers.ScaledDummy(20.0f);
        }
    }
}
