using System.Collections.Generic;
using System.Drawing;
using System.Numerics;
using Dalamud.Interface;
using Dalamud.Interface.Utility;
using Dalamud.Utility;
using ImGuiNET;
using KamiLib.CommandManager;
using KamiLib.Components;
using KamiLib.Extensions;
using KamiLib.Window;
using NoTankYou.Classes;
using Strings = NoTankYou.Localization.Strings;

namespace NoTankYou.Windows;

public class ConfigurationWindow : TabbedSelectionWindow<ModuleBase> {
    protected override List<ModuleBase> Options { get; }
    protected override List<ITabItem> Tabs { get; } = [
        new GeneralSettingsTab(),
    ];

    protected override float SelectionListWidth { get; set; } = 175.0f * ImGuiHelpers.GlobalScale;
    protected override float SelectionItemHeight { get; } = 32.0f * ImGuiHelpers.GlobalScale;
    protected override string SelectionListTabName => Strings.Modules;

    public ConfigurationWindow() : base("NoTankYou - Configuration Window", new Vector2(500.0f, 475.0f)) {
        Options = System.ModuleController.Modules;
        
        System.CommandManager.RegisterCommand(new CommandHandler {
            Delegate = OpenConfigWindow,
            ActivationPath = "/",
        });
    }

    protected override void DrawListOption(ModuleBase option) {
        var cursorStart = ImGui.GetCursorPos();
        var iconInfo = option.ModuleName.GetAttribute<ModuleIconAttribute>()!;
        if (iconInfo.BackgroundIcon is not 0) {
            ImGui.Image(Service.TextureProvider.GetFromGameIcon(iconInfo.BackgroundIcon).GetWrapOrEmpty().ImGuiHandle, ImGuiHelpers.ScaledVector2(32.0f, 32.0f));
        }
        
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
