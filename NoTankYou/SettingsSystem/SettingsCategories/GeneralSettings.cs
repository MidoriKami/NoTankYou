using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Dalamud.Interface.Components;
using ImGuiNET;
using NoTankYou.SettingsSystem.SubSettings;
using NoTankYou.SettingsSystem.SubSettings.General;

namespace NoTankYou.SettingsSystem.SettingsCategories
{
    internal class GeneralSettings : SettingsCategory
    {
        private readonly GeneralBasicSettings BasicSettings = new();
        private readonly ModeSelectSettings ModeSelectSettings = new();
        private readonly SelectSizeSettings SelectSizeSettings = new();
        private readonly RepositionSettings RepositionSettings = new();
        private readonly EnableDisableAllSettings EnableDisableAllSettings = new();

        public GeneralSettings() : base("General Settings")
        {

        }

        protected override void DrawContents()
        {
            ImGui.BeginChildFrame(1, new Vector2(440, 365), ImGuiWindowFlags.NoBackground);

            BasicSettings.Draw();

            ModeSelectSettings.Draw();

            SelectSizeSettings.Draw();

            RepositionSettings.Draw();

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
                ImGui.TableNextColumn();
                ImGui.Text("Tank Stance");

                ImGui.TableNextColumn();
                DrawConditionalText(Service.Configuration.TankStanceSettings.Enabled, "Enabled", "Disabled");

                ImGui.TableNextColumn();
                ImGui.Text("Dance Partner");

                ImGui.TableNextColumn();
                DrawConditionalText(Service.Configuration.DancePartnerSettings.Enabled, "Enabled", "Disabled");

                ImGui.TableNextColumn();
                ImGui.Text("Faerie");

                ImGui.TableNextColumn();
                DrawConditionalText(Service.Configuration.FaerieSettings.Enabled, "Enabled", "Disabled");

                ImGui.TableNextColumn();
                ImGui.Text("Kardion");

                ImGui.TableNextColumn();
                DrawConditionalText(Service.Configuration.KardionSettings.Enabled, "Enabled", "Disabled");

                ImGui.TableNextColumn();
                ImGui.Text("Summoner Pet");

                ImGui.TableNextColumn();
                DrawConditionalText(Service.Configuration.SummonerSettings.Enabled, "Enabled", "Disabled");

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
