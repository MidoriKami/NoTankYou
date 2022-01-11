using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Dalamud.Interface.Components;
using ImGuiNET;

namespace NoTankYou.SettingsSystem.SettingsCategories
{
    internal class FoodSettings : BannerSettings
    {
        private int ModifyWhitelistValue;

        public FoodSettings()
        {
            CategoryName = "Food Warning Settings";
            TabName = "Food";
        }

        protected override ref Configuration.ModuleSettings Settings => ref Service.Configuration.FoodSettings;

        protected override void DrawContents()
        {
            ImGui.BeginChildFrame(2, new Vector2(490, 365), ImGuiWindowFlags.NoBackground);

            ImGui.TextColored(new Vector4(0, 250, 0, 1.0f), "Food Warning will only show in Whitelisted Areas");
            ImGui.Separator();
            ImGui.Spacing();

            PrintWhitelist();

            PrintAddRemoveCurrentTerritoryWhitelist();

            PrintAddRemoveManualTerritoryWhitelist();

            DrawEarlyWarningTime();

            DrawFoodSoloModeCheckbox();

            base.DrawContents();

            ImGui.EndChildFrame();
        }

        private static void PrintWhitelist()
        {
            ImGui.Text("Currently Whitelisted Areas");
            ImGui.Spacing();

            if (Service.Configuration.FoodTerritoryWhitelist.Count > 0)
            {
                var whitelist = Service.Configuration.FoodTerritoryWhitelist;
                ImGui.Text("{" + string.Join(", ", whitelist) + "}");
            }
            else
            {
                ImGui.TextColored(new Vector4(180, 0, 0, 0.8f), "Whitelist is empty");
            }

            ImGui.Spacing();

        }
        private static void PrintAddRemoveCurrentTerritoryWhitelist()
        {
            ImGui.Text("Whitelist Operations");
            ImGui.Separator();
            ImGui.Spacing();

            ImGui.Text($"Currently in MapID: [{Service.ClientState.TerritoryType}]");
            ImGui.Spacing();

            if (ImGui.Button("Add Here", new(125, 25)))
            {
                var whitelist = Service.Configuration.FoodTerritoryWhitelist;

                if (!whitelist.Contains(Service.ClientState.TerritoryType))
                {
                    whitelist.Add(Service.ClientState.TerritoryType);
                    Service.Configuration.ForceWindowUpdate = true;
                }
            }

            ImGui.SameLine();

            if (ImGui.Button("Remove Here", new(125, 25)))
            {
                var whitelist = Service.Configuration.FoodTerritoryWhitelist;

                if (whitelist.Contains(Service.ClientState.TerritoryType))
                {
                    whitelist.Remove(Service.ClientState.TerritoryType);
                    Service.Configuration.ForceWindowUpdate = true;
                }
            }

            ImGuiComponents.HelpMarker("Adds or Removes the current map to or from the blacklist.");

            ImGui.Spacing();
        }
        private void PrintAddRemoveManualTerritoryWhitelist()
        {
            ImGui.Text("Manually Add or Remove");
            ImGui.Spacing();

            ImGui.PushItemWidth(150);
            ImGui.InputInt("##AddToWhitelist", ref ModifyWhitelistValue, 0, 0);
            ImGui.PopItemWidth();

            ImGui.SameLine();

            if (ImGui.Button("Add", new(75, 25)))
            {
                AddToWhitelist(ModifyWhitelistValue);
            }

            ImGui.SameLine();

            if (ImGui.Button("Remove", new(75, 25)))
            {
                RemoveFromWhitelist(ModifyWhitelistValue);
            }

            ImGui.SameLine();

            ImGuiComponents.HelpMarker("Adds or Removes specified map to or from the Whitelist");

            ImGui.Spacing();
        }
        private static void RemoveFromWhitelist(int territory)
        {
            var blacklist = Service.Configuration.FoodTerritoryWhitelist;

            if (blacklist.Contains(territory))
            {
                blacklist.Remove(territory);
                Service.Configuration.ForceWindowUpdate = true;
            }
        }
        private static void AddToWhitelist(int territory)
        {
            var blacklist = Service.Configuration.FoodTerritoryWhitelist;

            if (!blacklist.Contains(territory))
            {
                blacklist.Add(territory);
                Service.Configuration.ForceWindowUpdate = true;
            }
        }
        private static void DrawEarlyWarningTime()
        {
            ImGui.PushItemWidth(60);
            ImGui.InputFloat("Early Warning Time (Seconds)", ref Service.Configuration.FoodEarlyWarningTime, 0, 0);
            ImGui.PopItemWidth();
            ImGuiComponents.HelpMarker("How many seconds before food expires to show a warning for.\n" +
                                       "Minimim: 0\n" +
                                       "Maximum: 5400");

            if (Service.Configuration.FoodEarlyWarningTime < 0)
            {
                Service.Configuration.FoodEarlyWarningTime = 0;
            }

            if (Service.Configuration.FoodEarlyWarningTime > 5400)
            {
                Service.Configuration.FoodEarlyWarningTime = 5400;
            }

            ImGui.Spacing();
        }

        private static void DrawFoodSoloModeCheckbox()
        {
            ImGui.Checkbox("Food Solo Mode", ref Service.Configuration.FoodSoloMode);
            ImGuiComponents.HelpMarker("Only Show Food Warnings for self");
        }
    }
}
