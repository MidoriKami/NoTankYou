using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Dalamud.Interface;
using Dalamud.Interface.Components;
using Dalamud.Logging;
using ImGuiNET;
using Lumina.Excel.GeneratedSheets;

namespace NoTankYou.SettingsSystem.SettingsCategories
{
    internal class FoodSettings : BannerSettings
    {
        private readonly List<ContentFinderCondition> ContentFinderConditionList;
        private readonly HashSet<string> ContentTypeList;
        private HashSet<string> InstanceNames;
        private string SelectedContentTypeString = "";
        private string SelectedInstanceName = "";

        private int ModifyWhitelistValue;

        public FoodSettings()
        {
            CategoryName = "Food Warning Settings";
            TabName = "Food";

            ContentFinderConditionList = Service.DataManager.GetExcelSheet<ContentFinderCondition>()!.ToList();

            ContentTypeList = ContentFinderConditionList
                !.Where(c => c.ContentType.Value?.Name != null)
                .Select(c => c.ContentType.Value!.Name.ToString())
                .ToHashSet();

        }

        protected override ref Configuration.ModuleSettings Settings => ref Service.Configuration.FoodSettings;

        protected override void DrawContents()
        {
            ImGui.BeginChildFrame(2, ImGuiHelpers.ScaledVector2(490, 365), ImGuiWindowFlags.NoBackground);

            DrawFoodSoloModeCheckbox();

            if (Service.Configuration.FoodSoloMode == false)
            {
                ImGui.TextColored(new Vector4(0, 250, 0, 1.0f), "Food Warning will only show in Whitelisted Areas");
                ImGui.Separator();
                ImGui.Spacing();

                PrintWhitelist();

                PrintAddRemoveCurrentTerritoryWhitelist();

                PrintAddRemoveManualTerritoryWhitelist();

                PrintAddRemoveManualWithNameTerritoryWhitelist();
            }

            ImGui.Text("Food Specific Settings");
            ImGui.Separator();
            ImGui.Spacing();

            DrawEarlyWarningTime();


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

            if (ImGui.Button("Add Here", ImGuiHelpers.ScaledVector2(125, 25)))
            {
                var whitelist = Service.Configuration.FoodTerritoryWhitelist;

                if (!whitelist.Contains(Service.ClientState.TerritoryType))
                {
                    whitelist.Add(Service.ClientState.TerritoryType);
                    Service.Configuration.ForceWindowUpdate = true;
                }
            }

            ImGui.SameLine();

            if (ImGui.Button("Remove Here", ImGuiHelpers.ScaledVector2(125, 25)))
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

            ImGui.PushItemWidth(150 * ImGuiHelpers.GlobalScale);
            ImGui.InputInt("##AddToWhitelist", ref ModifyWhitelistValue, 0, 0);
            ImGui.PopItemWidth();

            ImGui.SameLine();

            if (ImGui.Button("Add", ImGuiHelpers.ScaledVector2(75, 25)))
            {
                AddToWhitelist(ModifyWhitelistValue);
            }

            ImGui.SameLine();

            if (ImGui.Button("Remove", ImGuiHelpers.ScaledVector2(75, 25)))
            {
                RemoveFromWhitelist(ModifyWhitelistValue);
            }

            ImGui.SameLine();

            ImGuiComponents.HelpMarker("Adds or Removes specified map to or from the Whitelist");

            ImGui.Spacing();
        }

        private void PrintAddRemoveManualWithNameTerritoryWhitelist()
        {

            ImGui.Text("Add or Remove by DutyFinder Name");
            ImGui.Separator();
            ImGui.Spacing();

            if (ImGui.BeginCombo("##ContentTypeSelection", SelectedContentTypeString))
            {
                foreach (var name in ContentTypeList)
                {
                    bool isSelected = name == SelectedContentTypeString;
                    if (ImGui.Selectable(name, isSelected))
                    {
                        SelectedContentTypeString = name;
                        InstanceNames = ContentFinderConditionList
                            .Where(c => c.ContentType.Value != null)
                            .Where(c => c.Name != null)
                            .Where(c =>  c.ContentType.Value!.Name == SelectedContentTypeString)
                            .Select(c => c.Name.ToString())
                            .ToHashSet();
                    }

                    if (isSelected)
                    {
                        ImGui.SetItemDefaultFocus();
                    }
                }

                ImGui.EndCombo();
            }
            ImGui.Spacing();

            if (SelectedContentTypeString != "")
            {
                if (ImGui.BeginCombo("##TerritorySelectByName", SelectedInstanceName))
                {

                    foreach (var instanceName in InstanceNames)
                    {
                        bool isSelected = instanceName == SelectedInstanceName;
                        if (ImGui.Selectable(instanceName, isSelected))
                        {
                            SelectedInstanceName = instanceName;
                        }

                        if (isSelected)
                        {
                            ImGui.SetItemDefaultFocus();
                        }
                    }

                    ImGui.EndCombo();
                }

                ImGui.Spacing();


                if (ImGui.Button("Add Instance", ImGuiHelpers.ScaledVector2(125, 25)))
                {
                    var whitelist = Service.Configuration.FoodTerritoryWhitelist;

                    var instanceId = (int)ContentFinderConditionList
                        .Where(c => c.Name != null)
                        .First(c => c.Name.ToString() == SelectedInstanceName)
                        .TerritoryType.Value!.RowId;

                    if (!whitelist.Contains(instanceId))
                    {
                        whitelist.Add(instanceId);
                        Service.Configuration.ForceWindowUpdate = true;
                    }
                }

                ImGui.SameLine();

                if (ImGui.Button("Remove Instance", ImGuiHelpers.ScaledVector2(125, 25)))
                {
                    var whitelist = Service.Configuration.FoodTerritoryWhitelist;

                    var instanceId = (int)ContentFinderConditionList
                        .Where(c => c.Name != null)
                        .First(c => c.Name.ToString() == SelectedInstanceName)
                        .TerritoryType.Value!.RowId;

                    if (whitelist.Contains(instanceId))
                    {
                        whitelist.Remove(instanceId);
                        Service.Configuration.ForceWindowUpdate = true;
                    }
                }
            }

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
            ImGui.PushItemWidth(60 * ImGuiHelpers.GlobalScale);
            ImGui.InputFloat("Early Warning Time (Seconds)", ref Service.Configuration.FoodEarlyWarningTime, 0, 0);
            ImGui.PopItemWidth();
            ImGuiComponents.HelpMarker("How many seconds before food expires to show a warning for.\n" +
                                       "Minimum: 0\n" +
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
