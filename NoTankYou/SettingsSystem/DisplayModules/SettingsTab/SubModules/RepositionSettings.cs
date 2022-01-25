using System.Linq;
using System.Numerics;
using Dalamud.Interface;
using ImGuiNET;

namespace NoTankYou.SettingsSystem.DisplayModules.SettingsTab.SubModules
{
    internal class RepositionSettings : SettingsCategory
    {
        private bool CurrentlyRepositioningAll = false;
        
        public RepositionSettings() : base("Reposition Banner Settings")
        {
            CurrentlyRepositioningAll = SettingsModules.Modules.Values.All(element => element.Reposition == true);
        }

        protected override void DrawContents()
        {
            DrawRepositionAll();
            DrawRepositionAllStatus();
            DrawEnableSnapping();
            DrawSnappingHint();
        }

        private void DrawRepositionAll()
        {
            if (ImGui.Button("Enable Reposition All", ImGuiHelpers.ScaledVector2(150, 25)))
            {
                foreach(var (name, element) in SettingsModules.Modules)
                {
                    element.Reposition = true;
                }

                CurrentlyRepositioningAll = true;
            }

            ImGui.SameLine();

            if (ImGui.Button("Disable Reposition All", ImGuiHelpers.ScaledVector2(150, 25)))
            {
                foreach (var (name, element) in SettingsModules.Modules)
                {
                    element.Reposition = false;
                }

                CurrentlyRepositioningAll = false;
            }

            ImGui.Spacing();
        }

        private void DrawRepositionAllStatus()
        {
            ImGui.Text("Reposition all: ");
            ImGui.SameLine();

            if (CurrentlyRepositioningAll)
            {
                ImGui.TextColored(new Vector4(0, 255, 0, 0.8f), "Enabled");
            }
            else
            {
                ImGui.TextColored(new Vector4(185, 0, 0, 0.8f), "Disabled");
            }

            ImGui.Spacing();
        }

        private void DrawEnableSnapping()
        {
            ImGui.Checkbox("Enable Window Snapping", ref Service.Configuration.WindowSnappingEnabled);
            ImGui.Spacing();
        }

        private void DrawSnappingHint()
        {
            ImGui.TextColored(new Vector4(120, 0, 0, 0.4f), "Press and hold [Control] while dragging a banner to snap");
            ImGui.Spacing();
        }
    }
}
