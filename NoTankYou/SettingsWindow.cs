using Dalamud.Interface.Components;
using Dalamud.Interface.Windowing;
using ImGuiNET;
using System;
using System.Numerics;

namespace NoTankYou
{
    internal class SettingsWindow : Window
    {
        private Tab CurrentTab = Tab.General;
        private readonly Vector2 WindowSize = new(400, 200);
        private int AddToBlacklistValue;
        private int RemoveFromBlacklistValue;

        public SettingsWindow() : base("No Tank You Settings Window")
        {
            IsOpen = false;

            SizeConstraints = new WindowSizeConstraints()
            {
                MinimumSize = new(WindowSize.X, WindowSize.Y),
                MaximumSize = new(WindowSize.X + 300, WindowSize.Y + 400)
            };
        }

        private enum Tab
        {
            General,
            TankStance,
            Blacklist
        }

        public override void Draw()
        {
            if (!IsOpen) return;

            DrawTabs();

            switch (CurrentTab)
            {
                case Tab.General:
                    DrawGeneralTab();
                    break;

                case Tab.TankStance:
                    DrawTankStanceTab();
                    break;

                case Tab.Blacklist:
                    DrawBlacklistTab();
                    break;
            }

            DrawSaveAndCloseButtons();
        }

        private void DrawTabs()
        {
            if (ImGui.BeginTabBar("No Tank You Tab Toolbar", ImGuiTabBarFlags.NoTooltip))
            {
                if (ImGui.BeginTabItem("General"))
                {
                    CurrentTab = Tab.General;
                    ImGui.EndTabItem();
                }

                if (ImGui.BeginTabItem("Tank Stance"))
                {
                    CurrentTab = Tab.TankStance;
                    ImGui.EndTabItem();
                }

                if (ImGui.BeginTabItem("Blacklist"))
                {
                    CurrentTab = Tab.Blacklist;
                    ImGui.EndTabItem();
                }
            }
        }

        private void DrawGeneralTab()
        {
            DrawDisableInAllianceRaidCheckbox();

            DrawInstanceLoadDelayTimeTextField();

            DrawStatus();
        }

        private void DrawStatus()
        {
            ImGui.Text("Warning Statuses");

            ImGui.Separator();
            ImGui.Spacing();

            if (ImGui.BeginTable("##StatusTable", 2))
            {
                ImGui.TableNextColumn();
                ImGui.Text("Tank Stance");

                ImGui.TableNextColumn();
                DrawConditionalText(Service.Configuration.EnableTankStanceBanner, "Enabled", "Disabled");

                ImGui.EndTable();
            }
            ImGui.Spacing();

        }
        private void DrawDisableInAllianceRaidCheckbox()
        {
            ImGui.Checkbox("Disable in Alliance Raids", ref Service.Configuration.DiableInAllianceRaid);

            ImGui.Spacing();
        }
        private void DrawInstanceLoadDelayTimeTextField()
        {
            ImGui.Text("Grace Period");
            ImGui.InputInt("", ref Service.Configuration.TerritoryChangeDelayTime, 1000, 5000);
            ImGuiComponents.HelpMarker("Hide warnings on map change for (milliseconds)");
            ImGui.Spacing();
        }
        private void DrawTankStanceTab()
        {
            ImGui.Text("Tanks Stance Warning Settings");
            ImGui.Separator();
            ImGui.Spacing();

            ImGui.Checkbox("Enable Tank Stance Warning", ref Service.Configuration.EnableTankStanceBanner);
            ImGui.Spacing();

            ImGui.Checkbox("Force Show Banner", ref Service.Configuration.ForceShowTankStanceBanner);
            ImGui.Spacing();

            ImGui.Checkbox("Reposition Banner", ref Service.Configuration.RepositionModeTankStanceBanner);
            ImGui.Spacing();

        }
        private void DrawBlacklistTab()
        {
            ImGui.Text("Blacklist Settings");
            ImGui.Separator();
            ImGui.Spacing();

            DrawBlacklistSettings();

            ImGui.Spacing();

        }
        private void DrawBlacklistSettings()
        {
            ImGui.Spacing();
            PrintBlackList();
            PrintAddToBlacklist();
            PrintRemoveFromBlacklist();

            ImGui.Spacing();
        }
        private void PrintBlackList()
        {
            ImGui.Text("Currently Blacklisted: ");

            if (Service.Configuration.TerritoryBlacklist.Count > 0)
            {
                var blacklist = Service.Configuration.TerritoryBlacklist;
                ImGui.Text("{" + string.Join(", ", blacklist) + "}");
            }
            else
            {
                ImGui.Text("Blacklist is empty.");
            }

            ImGui.Text($"Currently In: {Service.ClientState.TerritoryType}");
        }
        private void PrintAddToBlacklist()
        {
            ImGui.Spacing();

            ImGui.Text("Add To BlackList");

            ImGui.PushItemWidth(150);

            ImGui.InputInt("##AddToBlacklist", ref AddToBlacklistValue, 0, 0);

            ImGui.PopItemWidth();

            ImGui.SameLine();

            if (ImGui.Button("Add", new(75, 25)))
            {
                AddToBlacklist();
            }
            ImGui.SameLine();

            ImGuiComponents.HelpMarker("Add specified territory to blacklist");
        }
        private void PrintRemoveFromBlacklist()
        {
            ImGui.Spacing();

            ImGui.Text("Remove from Blacklist");

            ImGui.PushItemWidth(150);

            ImGui.InputInt("##RemoveFromBlacklist", ref RemoveFromBlacklistValue, 0, 0);

            ImGui.PopItemWidth();
            ImGui.SameLine();

            if (ImGui.Button("Remove", new(75, 25)))
            {
                RemoveFromBlacklist();
            }
            ImGui.SameLine();

            ImGuiComponents.HelpMarker("Removes specified territory from blacklist");
        }
        private void RemoveFromBlacklist()
        {
            var blacklist = Service.Configuration.TerritoryBlacklist;

            if (blacklist.Contains(RemoveFromBlacklistValue))
            {
                blacklist.Remove(RemoveFromBlacklistValue);
                Service.Configuration.ForceWindowUpdate = true;
            }
        }
        private void AddToBlacklist()
        {
            var blacklist = Service.Configuration.TerritoryBlacklist;

            if (!blacklist.Contains(AddToBlacklistValue))
            {
                blacklist.Add(AddToBlacklistValue);
                Service.Configuration.ForceWindowUpdate = true;
            }
        }
        private void DrawSaveAndCloseButtons()
        {
            ImGui.Spacing();

            if (ImGui.Button("Save", new(100, 25)))
            {
                Service.Configuration.Save();
            }

            ImGui.SameLine(ImGui.GetWindowWidth() - 155);

            if (ImGui.Button("Save & Close", new(150, 25)))
            {
                Service.Configuration.Save();
                IsOpen = false;
            }

            ImGui.Spacing();
        }
        private void DrawConditionalText(bool condition, string trueString, string falseString)
        {
            if (condition)
            {
                ImGui.Text(trueString);
            }
            else
            {
                ImGui.Text(falseString);
            }
        }
        public void Dispose()
        {

        }
    }
}
