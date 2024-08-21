using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using Dalamud.Interface;
using Dalamud.Interface.Utility;
using Dalamud.Interface.Utility.Raii;
using Dalamud.Utility;
using ImGuiNET;
using KamiLib.Classes;
using KamiLib.CommandManager;
using KamiLib.Configuration;
using KamiLib.Extensions;
using KamiLib.Window;
using KamiLib.Window.SelectionWindows;
using Lumina.Excel.GeneratedSheets;
using NoTankYou.Classes;
using Strings = NoTankYou.Localization.Strings;

namespace NoTankYou.Windows;

public class ConfigurationWindow : TabbedSelectionWindow<ModuleBase> {
    protected override bool AllowChildScrollbar => true;

    protected override List<ModuleBase> Options { get; }
    protected override List<ITabItem> Tabs { get; } = [
        new GeneralSettingsTab(),
        new PartListSettingsTab(),
        new BannerSettingsTab(),
        new BlacklistTab(),
    ];

    protected override float SelectionListWidth { get; set; } = 175.0f * ImGuiHelpers.GlobalScale;
    protected override float SelectionItemHeight { get; } = 32.0f * ImGuiHelpers.GlobalScale;
    protected override string SelectionListTabName => Strings.Modules;

    public ConfigurationWindow() : base("NoTankYou - Configuration Window", new Vector2(500.0f, 580.0f)) {
        Options = System.ModuleController.Modules;
        
        TitleBarButtons.Add(new TitleBarButton {
            Click = _ => System.WindowManager.AddWindow(new ConfigurationManagerWindow(), WindowFlags.OpenImmediately),
            Icon = FontAwesomeIcon.Cog,
            ShowTooltip = () => ImGui.SetTooltip("Open Configuration Manager"),
            IconOffset = new Vector2(2.0f, 1.0f),
        });
        
        System.CommandManager.RegisterCommand(new CommandHandler {
            Delegate = OpenConfigWindow,
            ActivationPath = "/",
        });
    }

    protected override void DrawListOption(ModuleBase option) {
        var cursorStart = ImGui.GetCursorPos();
        var iconInfo = option.ModuleName.GetAttribute<ModuleIconAttribute>()!;
        
        ImGui.SetCursorPos(cursorStart);
        ImGui.Image(Service.TextureProvider.GetFromGameIcon(iconInfo.ModuleIcon).GetWrapOrEmpty().ImGuiHandle, ImGuiHelpers.ScaledVector2(32.0f, 32.0f));
        
        ImGui.SameLine();
        ImGui.SetCursorPosY(ImGui.GetCursorPosY() + 8.0f * ImGuiHelpers.GlobalScale);
        ImGui.TextColored(option.IsEnabled ? KnownColor.LawnGreen.Vector() : KnownColor.White.Vector(), option.ModuleName.GetDescription());
    }

    protected override void DrawSelectedOption(ModuleBase option) {
        option.DrawConfigUi();
    }
    
    private void OpenConfigWindow(params string[] args) {
        if (!Service.ClientState.IsLoggedIn) return;
        if (Service.ClientState.IsPvP) return;
            
        Toggle();
    }
}

public class GeneralSettingsTab : ITabItem {
    public string Name => Strings.GeneralOptions;
    
    public bool Disabled => false;

    public void Draw()
        => System.SystemConfig.DrawConfigUi();
}

public class PartListSettingsTab : ITabItem {
    public string Name => "Party List";
    
    public bool Disabled => false;

    public void Draw()
        => System.PartyListController.DrawConfigUi();
}

public class BannerSettingsTab : ITabItem {
    public string Name => "Banner";
    
    public bool Disabled => false;

    public void Draw()
        => System.BannerController.DrawConfigUi(); 
}

public class BlacklistTab : ITabItem {
    public string Name => "Zone Blacklist";
    
    public bool Disabled => false;

    public void Draw() {
        using (var _ = ImRaii.PushColor(ImGuiCol.ChildBg, KnownColor.OrangeRed.Vector() with { W = 0.15f }, System.BlacklistController.config.BlacklistedZones.Contains(Service.ClientState.TerritoryType))) {
            DrawAddRemovableTerritory(GetCurrentTerritory());
        }
        
        ImGui.Separator();
        
        DrawCurrentlyBlacklisted();
        
        DrawAddNewButton();
    }
    
    private void DrawAddRemovableTerritory(TerritoryType territory) {
        using (var _ = ImRaii.PushFont(UiBuilder.IconFont)) {
            if (System.BlacklistController.config.BlacklistedZones.Contains(territory.RowId)) {
                if (ImGui.Button($"{FontAwesomeIcon.Trash.ToIconString()}##removeZone{territory.RowId}", new Vector2(25.0f, 75.0f))) {
                    System.BlacklistController.config.BlacklistedZones.Remove(territory.RowId);
                    System.BlacklistController.config.Save();
                }
            }
            else {
                if (ImGui.Button($"{FontAwesomeIcon.Plus.ToIconString()}##addZone{territory.RowId}", new Vector2(25.0f, 75.0f))) {
                    System.BlacklistController.config.BlacklistedZones.Add(territory.RowId);
                    System.BlacklistController.config.Save();
                }
            }
        }
        
        ImGui.SameLine();
        
        territory.Draw(Service.DataManager, Service.TextureProvider);
    }
    
    private void DrawCurrentlyBlacklisted() {
        using var child = ImRaii.Child("blacklist_frame", new Vector2(ImGui.GetContentRegionAvail().X, ImGui.GetContentRegionAvail().Y - 25.0f * ImGuiHelpers.GlobalScale));
        if (!child) return;

        ImGuiClip.ClippedDraw(System.BlacklistController.config.BlacklistedZones.ToList(), zoneId => {
            if (Service.DataManager.GetExcelSheet<TerritoryType>()?.GetRow(zoneId) is { } territory) {
                DrawAddRemovableTerritory(territory);
            }
        }, 75.0f);
    }
    
    private void DrawAddNewButton() {
        using var child = ImRaii.Child("open_selector_frame", ImGui.GetContentRegionAvail());
        if (!child) return;

        using var _ = ImRaii.PushFont(UiBuilder.IconFont);
        if (ImGui.Button($"{FontAwesomeIcon.Plus.ToIconString()}", ImGui.GetContentRegionAvail())) {
            System.WindowManager.AddWindow(new TerritorySelectionWindow(Service.PluginInterface) {
                MultiSelectionCallback = selections => {
                    foreach (var selection in selections) {
                        System.BlacklistController.config.BlacklistedZones.Add(selection.RowId);
                        System.BlacklistController.config.Save();
                    }
                }
            });
        }
    }

    private static TerritoryType GetCurrentTerritory() 
        => Service.DataManager.GetExcelSheet<TerritoryType>()!.GetRow(Service.ClientState.TerritoryType)!;
}