using System.Numerics;
using Dalamud.Interface;
using ImGuiNET;
using NoTankYou.SettingsSystem.DisplayModules.SettingsTab.SubModules;

namespace NoTankYou.SettingsSystem.DisplayModules.SettingsTab
{
    internal class BannerSettings : DisplayModule
    {
        private readonly ModeSelectSettings ModeSelectSettings = new();
        private readonly EnableDisableAllSettings EnableDisableAllSettings = new();
        private readonly RepositionSettings RepositionSettings = new();
        private readonly SelectSizeSettings SelectSizeSettings = new();

        public BannerSettings()
        {
            CategoryString = "Banner Settings";
        }

        protected override void DrawContents()
        {
            ModeSelectSettings.Draw();

            EnableDisableAllSettings.Draw();

            RepositionSettings.Draw();

            SelectSizeSettings.Draw();

            DrawStatus();
        }

        public override void Dispose()
        {

        }

        private static void DrawStatus()
        {
            ImGui.Indent(-20 * ImGuiHelpers.GlobalScale);
            ImGui.Text("Warning Statuses");
            ImGui.Separator();
            ImGui.Spacing();
            ImGui.Indent(20 * ImGuiHelpers.GlobalScale);

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
