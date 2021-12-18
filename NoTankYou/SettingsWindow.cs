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
        private readonly Vector2 WindowSize = new(450, 485);
        private int ModifyBlacklistValue;
        private bool CurrentlyRepositioningAll = false;

        public SettingsWindow() : base("No Tank You Settings Window")
        {
            IsOpen = false;

            SizeConstraints = new WindowSizeConstraints()
            {
                MinimumSize = new(WindowSize.X, WindowSize.Y),
                MaximumSize = new(WindowSize.X, WindowSize.Y)
            };

            Flags |= ImGuiWindowFlags.NoResize;
            Flags |= ImGuiWindowFlags.NoScrollbar;
            Flags |= ImGuiWindowFlags.NoScrollWithMouse;
        }

        private enum Tab
        {
            General,
            TankStance,
            DancePartner,
            Faerie,
            Kardion,
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

                case Tab.DancePartner:
                    DrawDancePartnerTab();
                    break;

                case Tab.Faerie:
                    DrawFaerieTab();
                    break;

                case Tab.Kardion:
                    DrawKardionTab();
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

                if (ImGui.BeginTabItem("Dance Partner"))
                {
                    CurrentTab = Tab.DancePartner;
                    ImGui.EndTabItem();
                }

                if (ImGui.BeginTabItem("Faerie"))
                {
                    CurrentTab = Tab.Faerie;
                    ImGui.EndTabItem();
                }

                if (ImGui.BeginTabItem("Kardion"))
                {
                    CurrentTab = Tab.Kardion;
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
            DrawGeneralSettings();

            DrawStatus();
        }
        private void DrawGeneralSettings()
        {
            ImGui.Text("General Settings");
            ImGui.Separator();
            ImGui.Spacing();

            DrawEnabledDisableAll();

            DrawRespositionAll();

            DrawDisableInAllianceRaids();

            DrawChangeWaitFrameCount();

            DrawInstanceLoadDelayTimeTextField();

            ImGui.Spacing();
        }

        private static void DrawDisableInAllianceRaids()
        {
            ImGui.Checkbox("Disable in Alliance Raids", ref Service.Configuration.DiableInAllianceRaid);

            ImGui.Spacing();
        }

        private static void DrawChangeWaitFrameCount()
        {
            ImGui.PushItemWidth(50);
            ImGui.InputInt("Number of Wait Frames", ref Service.Configuration.NumberOfWaitFrames, 0, 0);
            ImGui.PopItemWidth();
            ImGuiComponents.HelpMarker("How many frames to wait between warning evaluations.\n" +
                "Higher values repesents a larger delay on updating warnings.\n" +
                "Recommend half your displays refresh rate.\n" +
                "Minimum: 5\n" +
                "Maximum: 144");
            ImGui.Spacing();
            
            if(Service.Configuration.NumberOfWaitFrames < 5)
            {
                Service.Configuration.NumberOfWaitFrames = 5;
            }

            if(Service.Configuration.NumberOfWaitFrames > 144)
            {
                Service.Configuration.NumberOfWaitFrames = 144;
            }
        }

        private static void DrawEnabledDisableAll()
        {
            if (ImGui.Button("Enable All", new(100, 25)))
            {
                Service.Configuration.EnableDancePartnerBanner = true;
                Service.Configuration.EnableKardionBanner = true;
                Service.Configuration.EnableFaerieBanner = true;
                Service.Configuration.EnableTankStanceBanner = true;
            }

            ImGui.SameLine();

            if (ImGui.Button("Disable All", new(100, 25)))
            {
                Service.Configuration.EnableDancePartnerBanner = false;
                Service.Configuration.EnableKardionBanner = false;
                Service.Configuration.EnableFaerieBanner = false;
                Service.Configuration.EnableTankStanceBanner = false;
            }

            ImGui.Spacing();
        }

        private void DrawRespositionAll()
        {
            ImGui.Text("Reposition All Banners");
            ImGui.Separator();
            ImGui.Spacing();

            if (ImGui.Button("Enable Reposition", new(150, 25)))
            {
                Service.Configuration.RepositionModeDancePartnerBanner = true;
                Service.Configuration.RepositionModeKardionBanner = true;
                Service.Configuration.RepositionModeFaerieBanner = true;
                Service.Configuration.RepositionModeTankStanceBanner = true;
                CurrentlyRepositioningAll = true;
            }

            ImGui.SameLine();

            if (ImGui.Button("Disable Reposition", new(150, 25)))
            {
                Service.Configuration.RepositionModeDancePartnerBanner = false;
                Service.Configuration.RepositionModeKardionBanner = false;
                Service.Configuration.RepositionModeFaerieBanner = false;
                Service.Configuration.RepositionModeTankStanceBanner = false;
                CurrentlyRepositioningAll = false;
            }

            ImGui.Spacing();

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

            ImGui.Separator();
            ImGui.Spacing();

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
                DrawConditionalText(Service.Configuration.EnableTankStanceBanner, "Enabled", "Disabled");

                ImGui.TableNextColumn();
                ImGui.Text("Dance Partner");

                ImGui.TableNextColumn();
                DrawConditionalText(Service.Configuration.EnableDancePartnerBanner, "Enabled", "Disabled");

                ImGui.TableNextColumn();
                ImGui.Text("Faerie");

                ImGui.TableNextColumn();
                DrawConditionalText(Service.Configuration.EnableFaerieBanner, "Enabled", "Disabled");

                ImGui.TableNextColumn();
                ImGui.Text("Kardion");

                ImGui.TableNextColumn();
                DrawConditionalText(Service.Configuration.EnableKardionBanner, "Enabled", "Disabled");

                ImGui.EndTable();
            }
            ImGui.Spacing();

        }
        private static void DrawInstanceLoadDelayTimeTextField()
        {
            ImGui.Text("Grace Period");
            ImGui.InputInt("", ref Service.Configuration.TerritoryChangeDelayTime, 1000, 5000);
            ImGuiComponents.HelpMarker("Hide warnings on map change for (milliseconds)\n" +
                "Recommended: 8,000 - 15,000\n" +
                "Minimum: 3,000");
            ImGui.Spacing();

            if(Service.Configuration.TerritoryChangeDelayTime < 3000)
            {
                Service.Configuration.TerritoryChangeDelayTime = 3000;
            }
        }
        private static void DrawTankStanceTab()
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
        private static void DrawDancePartnerTab()
        {
            ImGui.Text("Dance Partner Warning Settings");
            ImGui.Separator();
            ImGui.Spacing();

            ImGui.Checkbox("Enable Missing Dance Partner Warning", ref Service.Configuration.EnableDancePartnerBanner);
            ImGui.Spacing();

            ImGui.Checkbox("Force Show Banner", ref Service.Configuration.ForceShowDancePartnerBanner);
            ImGui.Spacing();

            ImGui.Checkbox("Reposition Banner", ref Service.Configuration.RepositionModeDancePartnerBanner);
            ImGui.Spacing();

        }
        private static void DrawFaerieTab()
        {
            ImGui.Text("Faerie Warning Settings");
            ImGui.Separator();
            ImGui.Spacing();

            ImGui.Checkbox("Enable Missing Faerie Warning", ref Service.Configuration.EnableFaerieBanner);
            ImGui.Spacing();

            ImGui.Checkbox("Force Show Banner", ref Service.Configuration.ForceShowFaerieBanner);
            ImGui.Spacing();

            ImGui.Checkbox("Reposition Banner", ref Service.Configuration.RepositionModeFaerieBanner);
            ImGui.Spacing();

            ImGui.Checkbox("Enable While Solo", ref Service.Configuration.EnableFaerieBannerWhileSolo);
            ImGuiComponents.HelpMarker("Enable while solo in a duty.");
            ImGui.Spacing();
        }
        private static void DrawKardionTab()
        {
            ImGui.Text("Kardion Warning Settings");
            ImGui.Separator();
            ImGui.Spacing();

            ImGui.Checkbox("Enable Missing Kardion Warning", ref Service.Configuration.EnableKardionBanner);
            ImGui.Spacing();

            ImGui.Checkbox("Force Show Banner", ref Service.Configuration.ForceShowKardionBanner);
            ImGui.Spacing();

            ImGui.Checkbox("Reposition Banner", ref Service.Configuration.RepositionModeKardionBanner);
            ImGui.Spacing();

            ImGui.Checkbox("Enable While Solo", ref Service.Configuration.EnableKardionBannerWhileSolo);
            ImGuiComponents.HelpMarker("Enable while solo in a duty.");
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
            PrintAddRemoveCurrentTerritoryBlacklist();
            PrintAddRemoveManualTerritoryBlacklist();

            ImGui.Spacing();
        }
        private static void PrintBlackList()
        {
            ImGui.Text("Currently Blacklisted Areas");
            ImGui.Spacing();

            if (Service.Configuration.TerritoryBlacklist.Count > 0)
            {
                var blacklist = Service.Configuration.TerritoryBlacklist;
                ImGui.Text("{" + string.Join(", ", blacklist) + "}");
            }
            else
            {
                ImGui.TextColored(new Vector4(180, 0, 0, 0.8f), "Blacklist is empty");
            }

            ImGui.Spacing();

        }
        private static void PrintAddRemoveCurrentTerritoryBlacklist()
        {
            ImGui.Text("Blacklist Operations");
            ImGui.Separator();
            ImGui.Spacing();

            ImGui.Text($"Currently in MapID: [{Service.ClientState.TerritoryType}]");
            ImGui.Spacing();

            if (ImGui.Button("Add Here", new(125, 25)))
            {
                var blacklist = Service.Configuration.TerritoryBlacklist;

                if (!blacklist.Contains(Service.ClientState.TerritoryType))
                {
                    blacklist.Add(Service.ClientState.TerritoryType);
                    Service.Configuration.ForceWindowUpdate = true;
                }
            }

            ImGui.SameLine();

            if (ImGui.Button("Remove Here", new(125, 25)))
            {
                var blacklist = Service.Configuration.TerritoryBlacklist;

                if (blacklist.Contains(Service.ClientState.TerritoryType))
                {
                    blacklist.Remove(Service.ClientState.TerritoryType);
                    Service.Configuration.ForceWindowUpdate = true;
                }
            }

            ImGuiComponents.HelpMarker("Adds or Removes the current map to or from the blacklist.");

            ImGui.Spacing();
        }
        private void PrintAddRemoveManualTerritoryBlacklist()
        {
            ImGui.Text("Manually Add or Remove");
            ImGui.Spacing();

            ImGui.PushItemWidth(150);
            ImGui.InputInt("##AddToBlacklist", ref ModifyBlacklistValue, 0, 0);
            ImGui.PopItemWidth();

            ImGui.SameLine();

            if (ImGui.Button("Add", new(75, 25)))
            {
                AddToBlacklist(ModifyBlacklistValue);
            }

            ImGui.SameLine();

            if (ImGui.Button("Remove", new(75, 25)))
            {
                RemoveFromBlacklist(ModifyBlacklistValue);
            }

            ImGui.SameLine();

            ImGuiComponents.HelpMarker("Adds or Removes specified map to or from the blacklist");

            ImGui.Spacing();
        }
        private static void RemoveFromBlacklist(int territory)
        {
            var blacklist = Service.Configuration.TerritoryBlacklist;

            if (blacklist.Contains(territory))
            {
                blacklist.Remove(territory);
                Service.Configuration.ForceWindowUpdate = true;
            }
        }
        private static void AddToBlacklist(int territory)
        {
            var blacklist = Service.Configuration.TerritoryBlacklist;

            if (!blacklist.Contains(territory))
            {
                blacklist.Add(territory);
                Service.Configuration.ForceWindowUpdate = true;
            }
        }
        private void DrawSaveAndCloseButtons()
        {
            ImGui.Spacing();

            var windowSize = ImGui.GetWindowSize();
            ImGui.SetCursorPos(new Vector2(5, windowSize.Y - 30));

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
        private static void DrawConditionalText(bool condition, string trueString, string falseString)
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
