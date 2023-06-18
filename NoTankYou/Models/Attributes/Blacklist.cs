using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using Dalamud.Interface;
using Dalamud.Interface.Components;
using Dalamud.Utility;
using ImGuiNET;
using KamiLib.AutomaticUserInterface;
using KamiLib.Caching;
using KamiLib.Utilities;
using Lumina.Excel.GeneratedSheets;
using NoTankYou.Localization;
using Action = System.Action;

namespace NoTankYou.Models.Attributes;

public class Blacklist : DrawableAttribute
{
    public class SearchResult
    {
        public uint TerritoryID { get; init; }
        private uint PlaceNameRow => LuminaCache<TerritoryType>.Instance.GetRow(TerritoryID)?.PlaceName.Row ?? 0;
        public string TerritoryName => LuminaCache<PlaceName>.Instance.GetRow(PlaceNameRow)?.Name.ToDalamudString().TextValue ?? "Unknown PlaceName Row";
    }
    
    private static string _searchString = string.Empty;
    private static List<SearchResult>? _searchResults = new();

    public Blacklist(string category, int group) : base(null, category, group)
    {
        _searchResults = Search("", 10);
    }
    
    protected override void Draw(object obj, FieldInfo field, Action? saveAction = null)
    {
        var hashSet = GetValue<HashSet<uint>>(obj, field);
        var removalSet = new HashSet<uint>();

        if (ImGui.InputTextWithHint($"##SearchBox", Strings.Search, ref _searchString, 64))
        {
            _searchResults = Search(_searchString, 10);
        }

        if (_searchResults is not null)
        {
            foreach (var result in _searchResults)
            {
                if (hashSet.Contains(result.TerritoryID))
                {
                    if (ImGuiComponents.IconButton($"RemoveButton{result.TerritoryID}", FontAwesomeIcon.Trash))
                    {
                        removalSet.Add(result.TerritoryID);
                    }
                }
                else
                {
                    if (ImGuiComponents.IconButton($"AddButton{result.TerritoryID}", FontAwesomeIcon.Plus))
                    {
                        AddZone(obj, field, saveAction, result.TerritoryID);
                    }
                }
                
                ImGui.SameLine();
                
                DrawTerritory(result.TerritoryID);
            }
        }
        
        ImGuiHelpers.ScaledDummy(10.0f);
        ImGui.Unindent(15.0f);
        ImGui.TextUnformatted(Strings.BlacklistedZones);
        ImGui.Separator();
        ImGui.Indent(15.0f);

        foreach (var zone in hashSet)
        {
            if (ImGuiComponents.IconButton($"RemoveButton{zone}", FontAwesomeIcon.Trash))
            {
                removalSet.Add(zone);
            }
            
            ImGui.SameLine();
            
            DrawTerritory(zone);
        }

        foreach (var removalZone in removalSet)
        {
            RemoveZone(obj, field, saveAction, removalZone);
        }
    }
    
    private void AddZone(object obj, FieldInfo field, Action? saveAction, uint zone)
    {
        var hashSet = GetValue<HashSet<uint>>(obj, field);
        hashSet.Add(zone);
        saveAction?.Invoke();
    }
    
    private void RemoveZone(object obj, FieldInfo field, Action? saveAction, uint zone)
    {
        var hashSet = GetValue<HashSet<uint>>(obj, field);
        hashSet.Remove(zone);
        saveAction?.Invoke();
    }
    
    private void DrawTerritory(uint zoneId)
    {
        var zone = LuminaCache<TerritoryType>.Instance.GetRow(zoneId);
        if (zone is null) return;

        var placeNameKey = zone.PlaceName.Row;
        var placeNameStringRow = LuminaCache<PlaceName>.Instance.GetRow(placeNameKey)!;
        var territoryName = placeNameStringRow.Name.ToDalamudString().ToString();
        
        if (ImGui.BeginTable($"##TerritoryTypeTable{zone.RowId}", 2))
        {
            ImGui.TableSetupColumn($"##TerritoryRow{zone.RowId}", ImGuiTableColumnFlags.WidthFixed, 50.0f);
            ImGui.TableSetupColumn($"##Label{zone.RowId}");

            ImGui.TableNextColumn();
            ImGui.TextColored(KnownColor.Gray.AsVector4(), zone.RowId.ToString());
            
            ImGui.TableNextColumn();
            ImGui.TextUnformatted(territoryName);
            
            ImGui.EndTable();
        }
    }
    
    private static List<SearchResult> Search(string searchTerms, int numResults)
    {
        return LuminaCache<TerritoryType>.Instance
            .Where(territory => territory.PlaceName.Row is not 0)
            .Where(territory => territory.PlaceName.Value is not null)
            .GroupBy(territory => territory.PlaceName.Value!.Name.ToDalamudString().TextValue)
            .Select(territory => territory.First())
            .Where(territory => territory.PlaceName.Value!.Name.ToDalamudString().TextValue.ToLower().Contains(searchTerms.ToLower()))
            .Select(territory => new SearchResult {
                TerritoryID = territory.RowId
            })
            .OrderBy(searchResult => searchResult.TerritoryName)
            .Take(numResults)
            .ToList();
    }
}
