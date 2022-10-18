using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Dalamud.Interface;
using Dalamud.Interface.Windowing;
using Dalamud.Utility;
using ImGuiNET;
using Lumina.Excel.GeneratedSheets;
using NoTankYou.Configuration;
using NoTankYou.Configuration.Components;
using NoTankYou.Localization;
using NoTankYou.UserInterface.Components.InfoBox;
using NoTankYou.Utilities;

namespace NoTankYou.UserInterface.Windows;

internal record SearchResult(uint TerritoryID, string TerritoryName, string TerritoryIntendedUse);

internal class BlacklistConfigurationWindow : Window, IDisposable
{
    private static BlacklistSettings Settings => Service.ConfigurationManager.CharacterConfiguration.Blacklist;

    private readonly InfoBox Options = new();
    private readonly InfoBox BlacklistStatus = new();
    private readonly InfoBox AddHere = new();
    private readonly InfoBox AddByID = new();
    private readonly InfoBox AddByName = new();

    private static int _modifyBlacklistValue;

    private static readonly HashSet<SearchResult> AllTerritories = new();
    private static readonly HashSet<string> CategoryList = new();
    private static HashSet<SearchResult> _instanceNames = new();
    private static string _selectedContentTypeString = "";
    private static SearchResult _selectedResult = new(0, "Unknown", "Unknown");

    public BlacklistConfigurationWindow() : base($"{Strings.TabItems.Blacklist.Label} - {Service.ConfigurationManager.CharacterConfiguration.CharacterData.Name}")
    {
        SizeConstraints = new WindowSizeConstraints
        {
            MinimumSize = new Vector2(275 * (4.0f / 3.0f), 350),
            MaximumSize = new Vector2(9999, 9999)
        };

        Flags |= ImGuiWindowFlags.AlwaysVerticalScrollbar;

        foreach (var row in Service.DataManager.GetExcelSheet<TerritoryType>()!)
            if (row.PlaceName?.Value?.Name != null && row.PlaceName.Value.Name != string.Empty)
            {
                var searchResult = new SearchResult(row.RowId, row.PlaceName.Value.Name.RawString,
                    TerritoryIntendedUseHelper.GetUseDescription(row.TerritoryIntendedUse));

                if (!AllTerritories.Any(p => p.TerritoryName == searchResult.TerritoryName))
                    AllTerritories.Add(searchResult);

                CategoryList.Add(searchResult.TerritoryIntendedUse);
            }

        Service.ConfigurationManager.OnCharacterDataAvailable += UpdateWindowTitle;
    }

    public void Dispose()
    {
        Service.ConfigurationManager.OnCharacterDataAvailable -= UpdateWindowTitle;
    }

    private void UpdateWindowTitle(object? sender, CharacterConfiguration e)
    {
        WindowName = $"{Strings.TabItems.Blacklist.Label} - {e.CharacterData.Name}";
    }

    public override void Draw()
    {
        Options
            .AddTitle(Strings.Common.Labels.Options)
            .AddConfigCheckbox(Strings.Configuration.Enable, Settings.Enabled)
            .Draw();

        if (Settings.Enabled.Value)
        {
            BlacklistStatus
                .AddTitle(Strings.TabItems.Blacklist.CurrentStatus)
                .AddAction(DisplayBlacklistedAreas)
                .Draw();

            AddHere
                .AddTitle(Strings.TabItems.Blacklist.Here)
                .AddAction(AddRemoveHere)
                .Draw();

            AddByID
                .AddTitle(Strings.TabItems.Blacklist.ID)
                .AddAction(AddRemoveByID)
                .Draw();

            AddByName
                .AddTitle(Strings.TabItems.Blacklist.Name)
                .AddAction(AddRemoveByName)
                .Draw();
        }
    }

    private void Add(uint id)
    {
        if (!Settings.BlacklistedZones.Contains(id))
        {
            Settings.BlacklistedZones.Add(id);
            Service.ConfigurationManager.Save();
        }
    }

    private void Remove(uint id)
    {
        if (Settings.BlacklistedZones.Contains(id))
        {
            Settings.BlacklistedZones.Remove(id);
            Service.ConfigurationManager.Save();
        }
    }

    private string GetTerritoryName(uint territory)
    {
        var row = Service.DataManager.GetExcelSheet<TerritoryType>()!.GetRow(territory);

        return row?.PlaceName.Value?.Name ?? "Invalid Name";
    }

    private string GetLabelForTerritory(uint territory)
    {
        return $"[ {territory}, {GetTerritoryName(territory)} ]";
    }

    private void DisplayBlacklistedAreas()
    {
        if (Settings.BlacklistedZones.Count > 0)
        {
            if (Settings.BlacklistedZones.Count > 5)
            {
                var region = ImGui.GetContentRegionAvail() * 0.80f;

                ImGui.BeginChild("TerritoryBlacklistChild", ImGuiHelpers.ScaledVector2(region.X, 100), true);
            }

            foreach (var territory in Settings.BlacklistedZones) ImGui.Text(GetLabelForTerritory(territory));

            if (Settings.BlacklistedZones.Count > 5) ImGui.EndChild();
        }
        else
        {
            ImGui.TextColored(Colors.SoftRed, Strings.TabItems.Blacklist.Empty);
        }
    }

    private void AddRemoveHere()
    {
        var currentTerritoryID = Service.ClientState.TerritoryType;

        ImGui.Text(Strings.TabItems.Blacklist.CurrentLocation.Format(GetLabelForTerritory(currentTerritoryID)));
        ImGui.Spacing();

        if (ImGui.Button(Strings.Commands.Add, ImGuiHelpers.ScaledVector2(60, 23))) Add(currentTerritoryID);

        ImGui.SameLine();

        if (ImGui.Button(Strings.Commands.Remove, ImGuiHelpers.ScaledVector2(60, 23))) Remove(currentTerritoryID);
    }

    private void AddRemoveByID()
    {
        ImGui.PushItemWidth(50 * ImGuiHelpers.GlobalScale);
        ImGui.InputInt("##AddToBlacklist", ref _modifyBlacklistValue, 0, 0);
        ImGui.PopItemWidth();

        ImGui.SameLine();

        if (ImGui.Button(Strings.Commands.Add, ImGuiHelpers.ScaledVector2(75, 23))) Add((uint) _modifyBlacklistValue);

        ImGui.SameLine();

        if (ImGui.Button(Strings.Commands.Remove, ImGuiHelpers.ScaledVector2(75, 23)))
            Remove((uint) _modifyBlacklistValue);
    }

    private void AddRemoveByName()
    {
        var region = ImGui.GetContentRegionAvail() * 0.80f;

        ImGui.SetNextItemWidth(region.X);
        if (ImGui.BeginCombo("##ContentTypeSelection", _selectedContentTypeString))
        {
            foreach (var searchResult in CategoryList.OrderBy(s => s))
            {
                var isSelected = searchResult == _selectedContentTypeString;
                if (ImGui.Selectable(searchResult, isSelected))
                {
                    _selectedContentTypeString = searchResult;

                    _instanceNames = AllTerritories
                        .Where(r => r.TerritoryIntendedUse == _selectedContentTypeString)
                        .ToHashSet();
                }

                if (isSelected) ImGui.SetItemDefaultFocus();
            }

            ImGui.EndCombo();
        }

        ImGui.Spacing();

        if (_selectedContentTypeString != string.Empty)
        {
            ImGui.SetNextItemWidth(region.X);
            if (ImGui.BeginCombo("##TerritorySelectByName", _selectedResult.TerritoryName))
            {
                foreach (var instanceName in _instanceNames.OrderBy(o => o.TerritoryName))
                {
                    var isSelected = instanceName == _selectedResult;
                    if (ImGui.Selectable(instanceName.TerritoryName, isSelected)) _selectedResult = instanceName;

                    if (isSelected) ImGui.SetItemDefaultFocus();
                }

                ImGui.EndCombo();
            }

            ImGui.Spacing();

            if (_selectedResult.TerritoryName != string.Empty)
            {
                if (ImGui.Button(Strings.Commands.Add, ImGuiHelpers.ScaledVector2(region.X / 2 - 10.0f, 25)))
                    Add(_selectedResult.TerritoryID);

                ImGui.SameLine();

                if (ImGui.Button(Strings.Commands.Remove, ImGuiHelpers.ScaledVector2(region.X / 2 - 10.0f, 25)))
                    Remove(_selectedResult.TerritoryID);
            }
        }
    }
}