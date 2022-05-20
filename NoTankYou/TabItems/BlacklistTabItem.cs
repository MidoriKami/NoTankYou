using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Dalamud.Interface;
using Dalamud.Utility;
using ImGuiNET;
using Lumina.Excel.GeneratedSheets;
using NoTankYou.Components;
using NoTankYou.Data.Components;
using NoTankYou.Enums;
using NoTankYou.Interfaces;
using NoTankYou.Localization;
using NoTankYou.Utilities;

namespace NoTankYou.TabItems
{
    internal class BlacklistTabItem : ITabItem
    {
        public ModuleType ModuleType => ModuleType.Blacklist;

        private static BlacklistSettings Settings => Service.Configuration.SystemSettings.Blacklist;

        private static readonly InfoBox Options = new()
        {
            Label = Strings.Common.Labels.Options,
            ContentsAction = () =>
            {
                if (ImGui.Checkbox(Strings.Configuration.Enable, ref Settings.Enabled))
                {
                    Service.Configuration.Save();
                }
            }
        };

        private static readonly InfoBox BlacklistStatus = new()
        {
            Label = Strings.TabItems.Blacklist.CurrentStatus,
            ContentsAction = () =>
            {
                if (Settings.Territories.Count > 0)
                {
                    if (Settings.Territories.Count > 5)
                    {
                        var region = ImGui.GetContentRegionAvail() * 0.80f;

                        ImGui.BeginChild("TerritoryBlacklistChild", ImGuiHelpers.ScaledVector2(region.X, 100), true);
                    }

                    foreach (var territory in Settings.Territories)
                    {
                        ImGui.Text(territory.ToString());
                    }

                    if (Settings.Territories.Count > 5)
                    {
                        ImGui.EndChild();
                    }
                }
                else
                {
                    ImGui.TextColored(Colors.SoftRed, Strings.TabItems.Blacklist.Empty);
                }
            }
        };

        private static readonly InfoBox AddHere = new()
        {
            Label = Strings.TabItems.Blacklist.Here,
            ContentsAction = () =>
            {
                var currentTerritoryID = Service.ClientState.TerritoryType;
                var territoryInfo = new SimpleTerritory(currentTerritoryID);

                ImGui.Text(Strings.TabItems.Blacklist.CurrentLocation.Format(territoryInfo));
                ImGui.Spacing();

                if (ImGui.Button(Strings.Commands.Add, ImGuiHelpers.ScaledVector2(60, 23)))
                {
                    Add(currentTerritoryID);
                }

                ImGui.SameLine();

                if (ImGui.Button(Strings.Commands.Remove, ImGuiHelpers.ScaledVector2(60, 23)))
                {
                    Remove(currentTerritoryID);
                }
            }
        };

        private static int _modifyBlacklistValue;

        private static readonly HashSet<SearchResult> AllTerritories = new();
        private static readonly HashSet<string> CategoryList = new();
        private static HashSet<SearchResult> instanceNames = new();
        private static string selectedContentTypeString = "";
        private static SearchResult selectedResult = new();

        private static readonly InfoBox AddById = new()
        {
            Label = Strings.TabItems.Blacklist.ID,
            ContentsAction = () =>
            {
                ImGui.PushItemWidth(50 * ImGuiHelpers.GlobalScale);
                ImGui.InputInt("##AddToBlacklist", ref _modifyBlacklistValue, 0, 0);
                ImGui.PopItemWidth();

                ImGui.SameLine();

                if (ImGui.Button(Strings.Commands.Add, ImGuiHelpers.ScaledVector2(75, 23)))
                {
                    Add((uint)_modifyBlacklistValue);
                }

                ImGui.SameLine();

                if (ImGui.Button(Strings.Commands.Remove, ImGuiHelpers.ScaledVector2(75, 23)))
                {
                    Remove((uint)_modifyBlacklistValue);
                }
            }
        };

        private static readonly InfoBox AddByName = new()
        {
            Label = Strings.TabItems.Blacklist.Name,
            ContentsAction = () =>
            {
                var region = ImGui.GetContentRegionAvail() * 0.80f;

                ImGui.SetNextItemWidth(region.X);
                if (ImGui.BeginCombo("##ContentTypeSelection", selectedContentTypeString))
                {
                    foreach (var searchResult in CategoryList.OrderBy(s => s))
                    {
                        bool isSelected = searchResult == selectedContentTypeString;
                        if (ImGui.Selectable(searchResult, isSelected))
                        {
                            selectedContentTypeString = searchResult;

                            instanceNames = AllTerritories
                                .Where(r => r.TerritoryIntendedUse == selectedContentTypeString)
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

                if (selectedContentTypeString != string.Empty)
                {
                    ImGui.SetNextItemWidth(region.X);
                    if (ImGui.BeginCombo("##TerritorySelectByName", selectedResult.TerritoryName))
                    {
                        foreach (var instanceName in instanceNames.OrderBy(o => o.TerritoryName))
                        {
                            bool isSelected = instanceName == selectedResult;
                            if (ImGui.Selectable(instanceName.TerritoryName, isSelected))
                            {
                                selectedResult = instanceName;
                            }

                            if (isSelected)
                            {
                                ImGui.SetItemDefaultFocus();
                            }
                        }

                        ImGui.EndCombo();
                    }

                    ImGui.Spacing();

                    if (selectedResult.TerritoryName != string.Empty)
                    {
                        if (ImGui.Button(Strings.Commands.Add, ImGuiHelpers.ScaledVector2(region.X / 2 - 10.0f, 25)))
                        {
                            Add(selectedResult.TerritoryID);
                        }

                        ImGui.SameLine();

                        if (ImGui.Button(Strings.Commands.Remove, ImGuiHelpers.ScaledVector2(region.X /2 - 10.0f, 25)))
                        {
                            Remove(selectedResult.TerritoryID);
                        }
                    }
                }
            }
        };

        public BlacklistTabItem()
        {
            foreach (var row in Service.DataManager.GetExcelSheet<TerritoryType>()!)
            {
                if (row.PlaceName?.Value?.Name != null && row.PlaceName.Value.Name != string.Empty)
                {
                    var searchResult = new SearchResult()
                    {
                        TerritoryID = row.RowId,
                        TerritoryName = row.PlaceName.Value.Name,
                        TerritoryIntendedUse = TerritoryIntendedUseHelper.GetUseDescription(row.TerritoryIntendedUse)
                    };

                    if (!AllTerritories.Any(p => p.TerritoryName == searchResult.TerritoryName))
                    {
                        AllTerritories.Add(searchResult);
                    }

                    CategoryList.Add(searchResult.TerritoryIntendedUse);
                }
            }
        }

        public void DrawTabItem()
        {
            ImGui.TextColored(Settings.Enabled ? Colors.SoftGreen : Colors.SoftRed, Strings.TabItems.Blacklist.Label);
        }

        public void DrawConfigurationPane()
        {
            if(ImGui.BeginChild("BlacklistTabItemChild", new Vector2(-1)))
            {
                ImGuiHelpers.ScaledDummy(10.0f);
                Options.DrawCentered();

                if (Settings.Enabled)
                {
                    ImGuiHelpers.ScaledDummy(30.0f);
                    AddHere.DrawCentered();

                    ImGuiHelpers.ScaledDummy(30.0f);
                    AddById.DrawCentered();

                    ImGuiHelpers.ScaledDummy(30.0f);
                    AddByName.DrawCentered();

                    ImGuiHelpers.ScaledDummy(30.0f);
                    BlacklistStatus.DrawCentered();
                }

                ImGuiHelpers.ScaledDummy(20.0f);
            }

            ImGui.EndChild();
        }


        private static void Add(uint id)
        {
            var blacklist = Settings.Territories;

            var territoryInfo = new SimpleTerritory(id);

            if (!blacklist.Contains(territoryInfo))
            {
                blacklist.Add(territoryInfo);
                Service.Configuration.Save();
            }
        }

        private static void Remove(uint id)
        {
            var blacklist = Settings.Territories;

            var territoryInfo = new SimpleTerritory(id);

            if (blacklist.Contains(territoryInfo))
            {
                blacklist.Remove(territoryInfo);
                Service.Configuration.Save();
            }
        }
    }
}
