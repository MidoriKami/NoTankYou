using Dalamud.Interface.Components;
using Dalamud.Interface.Windowing;
using ImGuiNET;
using System;
using System.Numerics;

namespace NoTankYou
{
    internal class SettingsWindow : Window
    {
        public bool visible = false;

        private Tab currentTab = Tab.General;

        private int addToBlacklist;
        private int removeFromBlacklist;

        private enum Tab
        {
            General,
            Blacklist
        }

        public SettingsWindow() :
            base("NoTankYou Settings")
        {
        }

        public override void Draw()
        {
            if (!IsOpen)
            {
                return;
            }
            else
            {
                DrawTabs();

                switch(currentTab)
                {
                    case Tab.General: 
                        DrawGeneralSettings(); 
                        break;

                    case Tab.Blacklist: 
                        DrawBlacklistSettings(); 
                        break;
                }

                DrawSaveAndCloseButtons();
            }
        }

        private void DrawTabs()
        {
            if(ImGui.BeginTabBar("NoTankYouSettingsToolbar", ImGuiTabBarFlags.NoTooltip))
            {
                if(ImGui.BeginTabItem("General"))
                {
                    currentTab = Tab.General;
                    ImGui.EndTabItem();
                }

                if(ImGui.BeginTabItem("Blacklist"))
                {
                    currentTab = Tab.Blacklist;
                    ImGui.EndTabItem();
                }
            }
        }

        private void DrawGeneralSettings()
        {
            ImGui.Spacing();
            DrawEnableClickThroughCheckbox();
            DrawDisableInAllianceRaid();
            DrawInstanceLoadDelayTimeTextField();
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

            ImGui.InputInt("##AddToBlacklist", ref addToBlacklist, 0, 0);

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

            ImGui.InputInt("##RemoveFromBlacklist", ref removeFromBlacklist, 0, 0);

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

            if (blacklist.Contains(removeFromBlacklist))
            {
                blacklist.Remove(removeFromBlacklist);
            }
        }

        private void AddToBlacklist()
        {
            var blacklist = Service.Configuration.TerritoryBlacklist;

            if (!blacklist.Contains(addToBlacklist))
            {
                blacklist.Add(addToBlacklist);
            }
        }

        private void DrawInstanceLoadDelayTimeTextField()
        {
            ImGui.Text("Grace Period");
            ImGui.InputInt("", ref Service.Configuration.InstanceLoadDelayTime, 1000, 5000);
            ImGuiComponents.HelpMarker("Hide warning banner on map change for (milliseconds)");
            ImGui.Spacing();
        }

        private void DrawEnableClickThroughCheckbox()
        {
            ImGui.Checkbox("Show/Move Warning Banner", ref Service.Configuration.ShowMoveWarningBanner);
            ImGuiComponents.HelpMarker("Force the Warning Banner to display and enables movement to position the window");
            ImGui.Spacing();
        }

        private void DrawDisableInAllianceRaid()
        {
            ImGui.Checkbox("Disable in Alliance Raid", ref Service.Configuration.DisableInAllianceRaid);
            ImGuiComponents.HelpMarker("Prevent the warning from showing while in an alliance raid");
            ImGui.Spacing();
        }

        private void DrawSaveAndCloseButtons()
        {
            ImGui.Spacing();

            if (ImGui.Button("Save", new(100, 25)))
            {
                Service.Configuration.Save();
            }

            ImGui.SameLine();

            if (ImGui.Button("Save & Close", new(150, 25)))
            {
                Service.Configuration.Save();
                IsOpen = false;
            }

            ImGui.Spacing();
        }

        public override void OnClose()
        {
            base.OnClose();
            Service.Configuration.Save();
        }
    }
}
