using ImGuiNET;
using NoTankYou.SettingsSystem.SubSettings.General;
using System.Numerics;
using Dalamud.Interface;

namespace NoTankYou.SettingsSystem.SettingsCategories
{
    internal class GeneralSettings : TabCategory
    {
        private readonly GeneralBasicSettings BasicSettings = new();
        private readonly ModeSelectSettings ModeSelectSettings = new();
        private readonly EnableDisableAllSettings EnableDisableAllSettings = new();

        public GeneralSettings()
        {
            CategoryName = "General Settings";
            TabName = "General";
        }

        protected override void DrawContents()
        {
            ImGui.BeginChildFrame(1, ImGuiHelpers.ScaledVector2(490, 365), ImGuiWindowFlags.NoBackground);

            BasicSettings.Draw();

            ModeSelectSettings.Draw();

            EnableDisableAllSettings.Draw();

            DrawStatus();

            ImGui.EndChildFrame();
        }

        private static void DrawStatus()
        {
            ImGui.Text("Warning Statuses");

            ImGui.Separator();
            ImGui.Spacing();

            if (ImGui.BeginTable("##StatusTable", 2))
            {
                foreach (var (name, module) in SettingsModules.Modules)
                {
                    ImGui.TableNextColumn();
                    ImGui.Text(name);

                    ImGui.TableNextColumn();
                    DrawConditionalText(module.Enabled, "Enabled", "Disabled");
                }

                ImGui.EndTable();
            }
            ImGui.Spacing();

        }

        private static void DrawConditionalText(bool condition, string trueString, string falseString)
        {
            if (condition)
            {
                ImGui.TextColored(new Vector4(0, 255, 0, 0.8f), trueString);
            }
            else
            {
                ImGui.TextColored(new Vector4(185, 0, 0, 0.8f), falseString);
            }
        }
    }
}
