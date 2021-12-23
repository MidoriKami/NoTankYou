using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Dalamud.Interface.Components;
using ImGuiNET;
using NoTankYou.SettingsSystem.SettingsCategories;

namespace NoTankYou.SettingsSystem.SubSettings
{
    internal class ModeSelectSettings : SettingsCategory
    {
        private int MainMode = 0;
        private int SubMode = 0;

        public ModeSelectSettings() : base("Mode Selection")
        {
            MainMode = (int)Service.Configuration.ProcessingMainMode;
            SubMode = (int)Service.Configuration.ProcessingSubMode;
        }

        protected override void DrawContents()
        {
            DrawModeSelect();
        }

        private void DrawModeSelect()
        {
            MainMode = (int)Service.Configuration.ProcessingMainMode;
            SubMode = (int)Service.Configuration.ProcessingSubMode;

            ImGui.RadioButton("Party Mode", ref MainMode, (int) Configuration.MainMode.Party);
            ImGuiComponents.HelpMarker("Checks entire party's status to display warnings");
            ImGui.SameLine();
            ImGui.Indent(200);
            ImGui.RadioButton("Solo Mode", ref MainMode, (int) Configuration.MainMode.Solo);
            ImGuiComponents.HelpMarker("Checks only your status to display warnings");
            ImGui.Indent(-200);

            if (MainMode == (int) Configuration.MainMode.Party)
            {
                ImGui.Spacing();

                ImGui.TextColored(new Vector4(120, 0, 0, 0.4f), "Use \"Solo Mode\" to enable warnings while in a Trust");
            }

            if (MainMode == (int) Configuration.MainMode.Solo)
            {
                ImGui.Spacing();
                ImGui.Indent(250);
                ImGui.RadioButton("Only In Duty", ref SubMode, (int) Configuration.SubMode.OnlyInDuty);
                ImGuiComponents.HelpMarker("Only instanced content");

                ImGui.Spacing();
                ImGui.RadioButton("Everywhere", ref SubMode, (int) Configuration.SubMode.Everywhere);
                ImGuiComponents.HelpMarker("Includes Open World and Towns");
                ImGui.Indent(-250);
            }

            Service.Configuration.ProcessingMainMode = (Configuration.MainMode)MainMode;
            Service.Configuration.ProcessingSubMode = (Configuration.SubMode)SubMode;

            ImGui.Spacing();
        }
    }
}
