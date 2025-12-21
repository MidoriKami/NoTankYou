using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Security.Authentication.ExtendedProtection;
using Dalamud.Bindings.ImGui;
using Dalamud.Interface;
using Dalamud.Interface.Utility;
using Dalamud.Interface.Utility.Raii;
using Dalamud.Utility;
using KamiLib.Classes;
using KamiLib.CommandManager;
using KamiLib.Configuration;
using KamiLib.Extensions;
using KamiLib.Window;
using KamiLib.Window.SelectionWindows;
using Lumina.Excel.Sheets;
using NoTankYou.Classes;

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

    protected override float SelectionListWidth => 175.0f;
    protected override float SelectionItemHeight => 32.0f;
    protected override string SelectionListTabName => "Modules";

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
        ImGui.Image(Services.TextureProvider.GetFromGameIcon(iconInfo.ModuleIcon).GetWrapOrEmpty().Handle, ImGuiHelpers.ScaledVector2(32.0f, 32.0f));
        
        ImGui.SameLine();
        ImGui.SetCursorPosY(ImGui.GetCursorPosY() + 8.0f * ImGuiHelpers.GlobalScale);
        ImGui.TextColored(option.IsEnabled ? KnownColor.LawnGreen.Vector() : KnownColor.White.Vector(), option.ModuleName.GetDescription());
    }

    protected override void DrawSelectedOption(ModuleBase option) {
        option.DrawConfigUi();
    }
    
    private void OpenConfigWindow(params string[] args) {
        if (!Services.ClientState.IsLoggedIn) return;
            
        Toggle();
    }

    public override void OnClose() {
        System.PartyListController.Config.SampleMode = false;
    }
}

public class GeneralSettingsTab : ITabItem {
    public string Name => "General Options";
    
    public bool Disabled => false;

    public void Draw() {
        var config = System.SystemConfig;
        if (config is null) {
            ImGui.TextColored(KnownColor.Orange.Vector(), "System Config Failed to Load.");
            return;
        }
            
        var configChanged = false;
		
        ImGuiTweaks.Header("Misc Options");
        using (ImRaii.PushIndent()) {
            configChanged |= ImGuiTweaks.Checkbox("Wait for Duty Start", ref config.WaitUntilDutyStart, "Waits for the starting ring to dissipate before showing warnings");
        }
		
        ImGuiTweaks.Header("Warning Display Options");
        using (ImRaii.PushIndent()) {
            configChanged |= ImGui.Checkbox("Hide in Quest Event", ref config.HideInQuestEvent);
        }

        if (configChanged) {
            Utilities.Config.SaveCharacterConfig(config,"System.config.json");
        }
    }
}

public class PartListSettingsTab : ITabItem {
    public string Name => "Party List";
    
    public bool Disabled => false;

    public void Draw()
        => System.PartyListController.DrawConfigUi();
}

public class BlacklistTab : ITabItem {
    public string Name => "Zone Blacklist";
    
    public bool Disabled => false;

    public void Draw() {
        using (var _ = ImRaii.PushColor(ImGuiCol.ChildBg, KnownColor.OrangeRed.Vector() with { W = 0.15f }, System.BlacklistController.Config.BlacklistedZones.Contains(Services.ClientState.TerritoryType))) {
            DrawAddRemovableTerritory(GetCurrentTerritory());
        }
        
        ImGui.Separator();
        
        DrawCurrentlyBlacklisted();
        
        DrawAddNewButton();
    }
    
    private void DrawAddRemovableTerritory(TerritoryType territory) {
        using (var _ = ImRaii.PushFont(UiBuilder.IconFont)) {
            if (System.BlacklistController.Config.BlacklistedZones.Contains(territory.RowId)) {
                if (ImGui.Button($"{FontAwesomeIcon.Trash.ToIconString()}##removeZone{territory.RowId}", ImGuiHelpers.ScaledVector2(25.0f, 75.0f))) {
                    System.BlacklistController.Config.BlacklistedZones.Remove(territory.RowId);
                    System.BlacklistController.Config.Save();
                }
            }
            else {
                if (ImGui.Button($"{FontAwesomeIcon.Plus.ToIconString()}##addZone{territory.RowId}", ImGuiHelpers.ScaledVector2(25.0f, 75.0f))) {
                    System.BlacklistController.Config.BlacklistedZones.Add(territory.RowId);
                    System.BlacklistController.Config.Save();
                }
            }
        }
        
        ImGui.SameLine();
        
        territory.Draw(Services.DataManager, Services.TextureProvider);
    }
    
    private void DrawCurrentlyBlacklisted() {
        using var child = ImRaii.Child("blacklist_frame", new Vector2(ImGui.GetContentRegionAvail().X, ImGui.GetContentRegionAvail().Y - 25.0f * ImGuiHelpers.GlobalScale));
        if (!child) return;

        ImGuiClip.ClippedDraw(System.BlacklistController.Config.BlacklistedZones.ToList(), zoneId => {
            if (Services.DataManager.GetExcelSheet<TerritoryType>().GetRow(zoneId) is var territory) {
                DrawAddRemovableTerritory(territory);
            }
        }, 75.0f);
    }
    
    private void DrawAddNewButton() {
        using var child = ImRaii.Child("open_selector_frame", ImGui.GetContentRegionAvail());
        if (!child) return;

        using var _ = ImRaii.PushFont(UiBuilder.IconFont);
        if (ImGui.Button($"{FontAwesomeIcon.Plus.ToIconString()}", ImGui.GetContentRegionAvail())) {
            System.WindowManager.AddWindow(new TerritorySelectionWindow(Services.PluginInterface) {
                MultiSelectionCallback = selections => {
                    foreach (var selection in selections) {
                        System.BlacklistController.Config.BlacklistedZones.Add(selection.RowId);
                        System.BlacklistController.Config.Save();
                    }
                },
            });
        }
    }

    private static TerritoryType GetCurrentTerritory() 
        => Services.DataManager.GetExcelSheet<TerritoryType>().GetRow(Services.ClientState.TerritoryType);
}